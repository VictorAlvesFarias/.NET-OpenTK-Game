using Kingdom_of_Creation.Comunication;
using Kingdom_of_Creation.Definitions;
using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Entities.Implements;
using Kingdom_of_Creation.Enums;
using Kingdom_of_Creation.Physics;
using Kingdom_of_Creation.Services.PolygonService.Implements;
using OpenTK.Graphics.OpenGL4;
using Server.Context.Camera.Implements;
using Server.Services.Implements;
using System.Diagnostics;
using System.Drawing.Imaging;

namespace Server
{
    public class ServerApplication
    {
        private readonly GameContext _gameContext;
        private readonly Physics _physics;
        private readonly PolygonService _polygonService;
        private readonly BroadcastService _broadcastService;

        // Server performance tracking
        private int _tickCount = 0;
        private double _lastTpsUpdate = 0;
        private double _currentTps = 0;
        private readonly Stopwatch _tpsStopwatch = Stopwatch.StartNew();

        public ServerApplication()
        {
            _gameContext = new GameContext();
            // Você pode personalizar as tolerâncias aqui se necessário
            // _physics = new Physics(positionTolerance: 0.002f, velocityTolerance: 0.002f);
            _physics = new Physics(); // Usa valores padrão: 0.001f para ambos
            _polygonService = new PolygonService();
            _broadcastService = new BroadcastService();
        }

        public async Task InitializeAsync()
        {
            Console.WriteLine("Server initialized. TPS will be displayed in the console.");
            Console.WriteLine("Starting server...");
            
            RegisterEvents();
            await InitializeMap();

            Program.IsRunning = true;

            _ = Task.Run(() => Program.UdpServer.StartAsync());
            await StartLoop();
        }
        private void RegisterEvents()
        {
            Program.UdpServer.On<HandleMoveEvent>("movement", async (data, client) =>
            {
                await HandlePlayerEvents(data.PlayerId, data.Event);
            });

            Program.UdpServer.On<RenderObject>("createPlatform", async (data, client) =>
            {
                await CreatePlatform(data.Position, data.Size);
            });

            Program.UdpServer.On<object>("requestInitPlayer", async (_, client) =>
            {
                await OnConnectionOpen();
            });

            // Ping handler
            Program.UdpServer.On<string>("ping", async (data, client) =>
            {
                var pongData = new PongResponse { Message = "pong", ServerTps = _currentTps };
                await Program.UdpServer.SendAsync(TcpMessage.FromObject("pong", pongData), client);
            });
        }
        private async Task InitializeMap()
        {

            var initPlatforms = new List<RenderObject>
            {
                new RenderObject(_polygonService.CreateRectangle(), PrimitiveType.Triangles)
                {
                    Position = new Vector_2(-0.5f, -0.5f),
                    Size = new Vector_2(5.0f, 0.1f),
                    Static = true
                },
                new RenderObject(_polygonService.CreateRectangle(), PrimitiveType.Triangles)
                {
                    Position = new Vector_2(-0.8f, -0.8f),
                    Size = new Vector_2(1.0f, 0.1f),
                    Static = true
                },
                new RenderObject(_polygonService.CreateRectangle(), PrimitiveType.Triangles)
                {
                    Position = new Vector_2(0f, -0.6f),
                    Size = new Vector_2(1.0f, 0.1f),
                    Static = true
                },
            };

            foreach (var item in initPlatforms)
            {
                item.OnPropertyChangeSubscriptions.Add(_broadcastService.SendRenderObject);
                _gameContext.MapObjects.Add(item);
                await item.OnPropertyChanged();
            }
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

                // Calculate TPS
                _tickCount++;
                double currentTpsTime = _tpsStopwatch.ElapsedMilliseconds;
                
                if (currentTpsTime - _lastTpsUpdate >= 1000) // Update every second
                {
                    _currentTps = _tickCount * 1000.0 / (currentTpsTime - _lastTpsUpdate);
                    _tickCount = 0;
                    _lastTpsUpdate = currentTpsTime;
                    
                    DisplayServerPerformanceInfo();
                }

                await Process(deltaTime);
            }
        }
        private async Task Process(float deltaTime)
        {
            float gravity = -9.8f;

            foreach (var item in _gameContext.MapObjects.Where(e=>e.Static == false).ToList()) 
            {
                _physics.ResolveColision(item, deltaTime, _gameContext.MapObjects.ToList(), gravity);

                if (item.ObjectChanged.Count > 0)
                {
                    await item.OnPropertyChanged();
                }
            }
        }
        private async Task HandlePlayerEvents(Guid playerId, PlayerEvents keyboardState)
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
        private async Task OnConnectionOpen()
        {

            var player = new Player();
            var obj = new RenderObject(_polygonService.CreateTriangle(), PrimitiveType.Triangles)
            {
                Position = new Vector_2(0, 0),
                Size = new Vector_2(0.2f, 0.2f),
                Speed = new Vector_2(4f, 4f),
                Id = Guid.NewGuid(),
                Static = false,
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

            player.OnPropertyChangeSubscriptions.Add(_broadcastService.SendPlayerObject);
            obj.OnPropertyChangeSubscriptions.Add(_broadcastService.SendRenderObject);

            _gameContext.ConnectedPlayers.Add(player);
            _gameContext.MapObjects.Add(obj);

            await _broadcastService.SendRenderObjects(_gameContext.MapObjects);
            await _broadcastService.SendPlayers(_gameContext.ConnectedPlayers);
            await _broadcastService.StartPlayer(player);
        }
        private async Task CreatePlatform(Vector_2 position, Vector_2 size)
        {
            var platform = new RenderObject(_polygonService.CreateCircle(), PrimitiveType.TriangleFan)
            {
                Position = position,
                Size = size,
                Static = false
            };

            platform.OnPropertyChangeSubscriptions.Add(_broadcastService.SendRenderObject);

            _gameContext.MapObjects.Add(platform);
        }

        private void DisplayServerPerformanceInfo()
        {
            Console.SetCursorPosition(0, 2);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"Server TPS: {_currentTps:F1}    ");
            Console.ResetColor();
        }
    }
}
