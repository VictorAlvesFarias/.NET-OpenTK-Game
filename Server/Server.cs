using Kingdom_of_Creation.Comunication;
using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Entities;
using Kingdom_of_Creation.Enums;
using OpenTK.Compute.OpenCL;
using System.Diagnostics;

namespace Server
{
    public static class ServerApplication
    {
        public static List<Player> _playerObject = new();
        public static List<Rectangle> _mapObjects = new();
        private static bool _isRunning = false;
        public static TcpServer _udpServer = new TcpServer(25565);

        public static async Task Main(string[] args)
        {
            Console.WriteLine("- Starting game server");

            _udpServer.On<HandleMoveEvent>("movement", async (data, client) =>
            {
                Console.WriteLine("[Server]: movement");

                HandleMove(data.PlayerId, data.Event);
            });
            _udpServer.On<Platform>("createPlatform", async (data, client) =>
            {
                Console.WriteLine("[Server]: createPlatform");

                CreatePlatform(data.Position, data.Size);
            });
            _udpServer.On<object>("requestInitPlayer", async (_, client) =>
            {
                Console.WriteLine("[Server]: requestInitPlayer");

                var newPlayer = OnConnectionOpen();

                var playerMsg = TcpMessage.FromObject("onConnectionOpened", newPlayer);
                await _udpServer.SendAsync(playerMsg, client);

                var mapMsg = TcpMessage.FromObject("onInit", _mapObjects);
                await _udpServer.SendAsync(mapMsg, client);
            });

            _mapObjects.Add(new Platform(new Vector_2(-0.5f, -0.5f), new Vector_2(5.0f, 0.1f)));
            _mapObjects.Add(new Platform(new Vector_2(-0.8f, -0.8f), new Vector_2(1.0f, 0.1f)));
            _mapObjects.Add(new Platform(new Vector_2(0f, -0.6f), new Vector_2(1.0f, 0.1f)));

            _isRunning = true;
            
            Console.WriteLine("- Starting server");

            _ = Task.Run(() => _udpServer.StartAsync());

            await StartLoop();
        }
        private static async Task StartLoop()
        {
            Console.WriteLine("- Starting loop");

            var stopwatch = new Stopwatch();
            var lastTime = stopwatch.ElapsedMilliseconds;
         
            stopwatch.Start();

            while (_isRunning)
            {
                Console.WriteLine("- Running");

                long currentTime = stopwatch.ElapsedMilliseconds;
                float deltaTime = (currentTime - lastTime) / 1000f;
                deltaTime = Math.Min(deltaTime, 0.1f);
                lastTime = currentTime;

                Process(deltaTime);

                var players = TcpMessage.FromObject("updatePlayers", _playerObject);
                var map = TcpMessage.FromObject("updateMap", _mapObjects);

                await _udpServer.BroadcastAsync(players);
                await _udpServer.BroadcastAsync(map);
            }
        }
        private static void Process(float deltaTime)
        {
            float gravity = -9.8f;

            _playerObject.ForEach((player) =>
            {

                if (!player.IsGrounded)
                {
                    player.Velocity = new Vector_2(player.Velocity.X, player.Velocity.Y + gravity * deltaTime );
                }

                Physics.ResolveColision(player.Velocity, player, deltaTime,_mapObjects);
            });
        }
        public static void HandleMove(Guid playerId, PlayerEvents keyboardState)
        {
            var player = _playerObject.Find(e => e.Id == playerId);

            if (keyboardState == PlayerEvents.Jump && player.IsGrounded)
            {
                player.Velocity = new Vector_2(player.Velocity.X, player.JumpForce);

                Console.WriteLine("Jump");
            }

            if (keyboardState == PlayerEvents.Left)
            {
                player.Velocity = new Vector_2(-player.Speed, player.Velocity.Y);

                Console.WriteLine("Left");
            }

            if (keyboardState == PlayerEvents.Right)
            {
                player.Velocity = new Vector_2(player.Speed , player.Velocity.Y);
            
                Console.WriteLine("Right");
            }

            if (keyboardState == PlayerEvents.Stop)
            {
                player.Velocity = new Vector_2(0, player.Velocity.Y);
            }
        }
        public static Player OnConnectionOpen()
        {
            var player = new Player(new Vector_2(0, 0), new Vector_2(0.2f, 0.2f), 4f, 2f, Guid.NewGuid());
            
            _playerObject.Add(player);

            return player;
        }
        public static void CreatePlatform(Vector_2 position, Vector_2 size)
        {
            Console.WriteLine(position);
            Console.WriteLine(size);
            _mapObjects.Add(new Platform(position, size));
        }
    }
}
