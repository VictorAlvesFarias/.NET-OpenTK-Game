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
        public static UdpServer _udpServer = new UdpServer(25565);

        public static void Main()
        {
            _udpServer.Subscribe(async (data, sender) =>
            {
                Console.WriteLine(data.Type);

                switch (data.Type)
                {
                    case "movement":
                        var moveEvent = data.Data.GetData<HandleMoveEvent>();
                        var player = _playerObject.FirstOrDefault(p => p.Id == moveEvent.PlayerId);

                        HandleMove(moveEvent.PlayerId, moveEvent.Event);

                        break;

                    case "createPlatform":
                        var platformData = data.Data.GetData<Platform>();
                        
                        CreatePlatform(platformData.Position, platformData.Size);
                        
                        break;

                    case "requestInitPlayer":
                        // Create new player for this client
                        var newPlayer = OnConnectionOpen();

                        foreach (var client in _udpServer._connectedClients)
                        {
                            var playerMsg = new UdpMessage()
                            {
                                Type = "onConnectionOpened",
                                Data = newPlayer.ToByte()
                            };

                            _udpServer.SendToAsync(playerMsg, client.Value);

                            var mapMsg = new UdpMessage()
                            {
                                Type = "onInit",
                                Data = _mapObjects.ToByte<List<Rectangle>>()
                            };

                            _udpServer.SendToAsync(mapMsg, client.Value);
                        }

                        break;

                    case "ping":
                        foreach (var client in _udpServer._connectedClients)
                        {
                            var playerMsg = new UdpMessage()
                            {
                                Type = "ping",
                                Data = []
                            };

                            _udpServer.SendToAsync(playerMsg, client.Value);
                        }
                            break;
                }
            });

            _mapObjects.Add(new Platform(new Vector_2(-0.5f, -0.5f), new Vector_2(5.0f, 0.1f)));
            _mapObjects.Add(new Platform(new Vector_2(-0.8f, -0.8f), new Vector_2(1.0f, 0.1f)));
            _mapObjects.Add(new Platform(new Vector_2(0f, -0.6f), new Vector_2(1.0f, 0.1f)));

            _isRunning = true;
            StartLoop();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }
        private static void StartLoop()
        {
            var thread = new Thread(() =>
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();

                long lastTime = stopwatch.ElapsedMilliseconds;

                while (_isRunning)
                {
                    long currentTime = stopwatch.ElapsedMilliseconds;
                    float deltaTime = (currentTime - lastTime) / 1000f;
                    deltaTime = Math.Min(deltaTime, 0.1f);
                    lastTime = currentTime;

                    Process(deltaTime);

                    var players = new UdpMessage()
                    {
                        Type = "updatePlayers",
                        Data = _playerObject.ToByte()
                    };
                    var platforms = new UdpMessage()
                    {
                        Type = "updateMap",
                        Data = _mapObjects.ToByte()
                    };

                    foreach (var client in _udpServer._connectedClients)
                    {
                        _udpServer.SendToAsync(platforms, client.Value);
                        _udpServer.SendToAsync(players, client.Value);
                    }

                    Thread.Sleep(1);
                }
            });

            thread.IsBackground = true;
            thread.Start();
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
