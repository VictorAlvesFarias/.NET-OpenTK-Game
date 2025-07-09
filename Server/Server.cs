using Kingdom_of_Creation.Comunication;
using Kingdom_of_Creation.Definitions;
using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Entities.Implements;
using Kingdom_of_Creation.Enums;
using Kingdom_of_Creation.Physics;
using Kingdom_of_Creation.Services.PolygonService.Implements;
using OpenTK.Graphics.OpenGL4;
using Server.Context.Camera.Implements;
using System.Diagnostics;

namespace Server
{
    public class ServerApplication
    {
        private readonly GameContext _gameContext;
        private readonly Physics _physics;
        private readonly PolygonService _polygonService;

        public ServerApplication()
        {
            _gameContext = new GameContext();
            _physics = new Physics();
            _polygonService = new PolygonService();
        }

        public async Task InitializeAsync()
        {
            RegisterEvents();
            InitializeMap();

            Program.IsRunning = true;

            _ = Task.Run(() => Program.UdpServer.StartAsync());
            await StartLoop();
        }
        private void RegisterEvents()
        {
            Program.UdpServer.On<HandleMoveEvent>("movement", async (data, client) =>
            {
                HandlePlayerEvents(data.PlayerId, data.Event);
            });

            Program.UdpServer.On<RenderObject>("createPlatform", async (data, client) =>
            {
                CreatePlatform(data.Position, data.Size);
            });

            Program.UdpServer.On<object>("requestInitPlayer", async (_, client) =>
            {
                var newPlayer = OnConnectionOpen();

                var playerMsg = TcpMessage.FromObject("onConnectionOpened", newPlayer);
                await Program.UdpServer.SendAsync(playerMsg, client);

                var mapMsg = TcpMessage.FromObject("onInit", _gameContext.MapObjects);
                await Program.UdpServer.SendAsync(mapMsg, client);
            });
        }
        private void InitializeMap()
        {
            var initPlatforms = new List<RenderObject>()
            {
                new RenderObject(_polygonService.CreateRectangle(), PrimitiveType.Triangles) 
                { 
                    Static = true, 
                    Position = new Vector_2(-0.5f, -0.5f), 
                    Size = new Vector_2(5.0f, 0.1f) 
                },
                new RenderObject(_polygonService.CreateRectangle(), PrimitiveType.Triangles) 
                { 
                    Static = true, 
                    Position = new Vector_2(-0.8f, -0.8f), 
                    Size = new Vector_2(1.0f, 0.1f) 
                },
                new RenderObject(_polygonService.CreateRectangle(), PrimitiveType.Triangles) 
                { 
                    Static = true, 
                    Position = new Vector_2(0f, -0.6f), 
                    Size = new Vector_2(1.0f, 0.1f) 
                }
            };

            _gameContext.MapObjects.AddRange(initPlatforms);
        }
        private async Task StartLoop()
        {
            var stopwatch = new Stopwatch();
            var lastTime = stopwatch.ElapsedMilliseconds;

            stopwatch.Start();

            while (Program.IsRunning)
            {
                long currentTime = stopwatch.ElapsedMilliseconds;
                float deltaTime = (currentTime - lastTime) / 1000f;
                deltaTime = Math.Min(deltaTime, 0.1f);
                lastTime = currentTime;

                Process(deltaTime);

                var players = TcpMessage.FromObject("updatePlayers", _gameContext.ConnectedPlayers);
                var map = TcpMessage.FromObject("updateMap", _gameContext.MapObjects);

                await Program.UdpServer.BroadcastAsync(players);
                await Program.UdpServer.BroadcastAsync(map);
            }
        }
        private void Process(float deltaTime)
        {
            float gravity = -9.8f;
            
            _gameContext.MapObjects.ToList().ForEach(obj =>
            {
                _physics.ResolveColision(obj, deltaTime, _gameContext.MapObjects.ToList(), gravity);
            });
        }
        private void HandlePlayerEvents(Guid playerId, PlayerEvents keyboardState)
        {
            var player = _gameContext.ConnectedPlayers.Find(e => e.Id == playerId);
            var renderObjectPlayer = _gameContext.MapObjects.Find(e => e.Id == player?.RenderObjectId);

            if (player == null) return;

            if (keyboardState == PlayerEvents.Jump && player.IsGrounded)
            {
                _physics.ApplyVelocity(renderObjectPlayer, new Vector_2(renderObjectPlayer.Velocity.X, renderObjectPlayer.Speed.Y));

                player.IsGrounded = false;
            }
            else if (keyboardState == PlayerEvents.Left)
            {
                _physics.ApplyVelocity(renderObjectPlayer, new Vector_2(-renderObjectPlayer.Speed.X, renderObjectPlayer.Velocity.Y));
            }
            else if (keyboardState == PlayerEvents.Right)
            {
                _physics.ApplyVelocity(renderObjectPlayer, new Vector_2(renderObjectPlayer.Speed.X, renderObjectPlayer.Velocity.Y));
            }
            else if (keyboardState == PlayerEvents.Stop)
            {
                _physics.ApplyVelocity(renderObjectPlayer, new Vector_2(0, renderObjectPlayer.Velocity.Y));
            }
            else if (keyboardState == PlayerEvents.Reset)
            {
                renderObjectPlayer.Position = new Vector_2(0, 0);
            }
        }
        private Player OnConnectionOpen()
        {

            var player = new Player();
            var obj = new RenderObject(_polygonService.CreateTriangle(), PrimitiveType.Triangles)
            {
                Position = new Vector_2(0, 0),
                Size = new Vector_2(0.2f, 0.2f),
                Speed = new Vector_2(4f, 4f),
                Id = Guid.NewGuid(),
                Color = ColorDefinitions.White,
            };

            player.RenderObjectId = obj.Id;

            _physics.ColisionEvents.Add(obj.Id, (renderObject, bot) =>
            {
                if (bot.HasCollision && bot.Normal.Y < 0.5f)
                {
                    player.IsGrounded = true;
                }
            });

            _gameContext.ConnectedPlayers.Add(player);
            _gameContext.MapObjects.Add(obj);

            return player;
        }
        private void CreatePlatform(Vector_2 position, Vector_2 size)
        {
            var platform = new RenderObject(_polygonService.CreateCircle(), PrimitiveType.TriangleFan)
            {
                Position = position,
                Size = size
            };

            _gameContext.MapObjects.Add(platform);
        }
    }
}
