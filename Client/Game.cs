using Kingdom_of_Creation.Enums;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using Kingdom_of_Creation.Comunication;
using Kingdom_of_Creation.Dtos;
using Client.Context.Camera.Implements;
using Client.Extensions;
using Kingdom_of_Creation.Entities.Implements;
using Client.Services;
using Kingdom_of_Creation.Services.PolygonService.Implements;
using System.Diagnostics;

namespace Client
{
    public class Game : GameWindow
    {
        private readonly CameraContext _cameraContext;
        private readonly GameContext _gameContext;
        private readonly RenderService _renderService;
        private readonly PolygonService _polygonService;

        // FPS tracking
        private int _frameCount = 0;
        private double _lastFpsUpdate = 0;
        private double _currentFps = 0;
        private readonly Stopwatch _fpsStopwatch = Stopwatch.StartNew();

        // Ping tracking
        private readonly Stopwatch _pingStopwatch = Stopwatch.StartNew();
        private double _lastPingUpdate = 0;
        private double _currentPing = 0;
        private bool _pingSent = false;
        private double _serverTps = 0;

        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title })
        {
            base.OnLoad();

            _cameraContext = new CameraContext(width, height, new Vector_2(0, 0), 0.3f, false);
            _gameContext = new GameContext();
            _renderService = new RenderService();
            _polygonService = new PolygonService();
        }

        protected override void OnFocusedChanged(FocusedChangedEventArgs e)
        {
            base.OnFocusedChanged(e);

            _cameraContext.WindowActive = e.IsFocused;
        }
        protected override void OnLoad()
        {
            base.OnLoad();

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            _cameraContext.UpdateProjectionMatrix();

            Program._udpClient.On<Player>("updatePlayer", data =>
            {
                var result = _gameContext.ConnectedPlayers.FirstOrDefault(e => e.Id == data.Id);

                if (result is null)
                {
                    _gameContext.ConnectedPlayers.Add(data);
                }
                else
                {
                    result.UpdateFrom(data);
                }
            });
            Program._udpClient.On<RenderObject>("updateObject", data =>
            {
                var result = _gameContext.MapObjects.FirstOrDefault(e => e.Id == data.Id);

                if (result is null)
                {
                    _gameContext.MapObjects.Add(data);
                }
                else
                {
                    result.UpdateFrom(data);
                }
            });
            Program._udpClient.On<List<RenderObject>>("loadObjects", data =>
            {
                _gameContext.MapObjects = data;
            });
            Program._udpClient.On<List<Player>>("loadPlayers", data =>
            {
                _gameContext.ConnectedPlayers = data;
            });
            Program._udpClient.On<Player>("setPlayer", data =>
            {
                _gameContext.PlayerObject = data;
            });

            // Ping handler
            Program._udpClient.On<PongResponse>("pong", data =>
            {
                if (_pingSent)
                {
                    _currentPing = _pingStopwatch.ElapsedMilliseconds;
                    _serverTps = data.ServerTps;
                    _pingSent = false;
                }
            });

            Program._udpClient.Connect("127.0.0.1", 25565);

            var initMessage = TcpMessage.FromObject("requestInitPlayer", null);

            Program._udpClient.SendAsync(initMessage);
        }
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);

            _cameraContext.UpdateWindowSize(e.Width, e.Height);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            // Calculate FPS
            _frameCount++;
            double currentTime = _fpsStopwatch.ElapsedMilliseconds;
            
            if (currentTime - _lastFpsUpdate >= 1000) // Update every second
            {
                _currentFps = _frameCount * 1000.0 / (currentTime - _lastFpsUpdate);
                _frameCount = 0;
                _lastFpsUpdate = currentTime;
                
                DisplayPerformanceInfo();
            }

            GL.Clear(ClearBufferMask.ColorBufferBit);
            Program.GetShader().Use();

            foreach (var obj in _gameContext.MapObjects.ToList())
            {
                _renderService.Draw(
                    obj.GetTransformedVertices(),
                    obj.Color,
                    obj.PrimitiveType
                );
            }

            if (_gameContext.IsDrawingPlatform && _gameContext.TempPlatform != null)
            {
                Vector_2 currentWorldPos = _cameraContext.ScreenToWorld(new Vector_2(MouseState.X, MouseState.Y));

                _gameContext.TempPlatform.CalculatePlatformGeometry(_gameContext.PlataformToAddedPosition, currentWorldPos);
               
                _renderService.Draw(
                    _gameContext.TempPlatform.GetTransformedVertices(),
                    _gameContext.TempPlatform.Color,
                    _gameContext.TempPlatform.PrimitiveType
                );
            }

            SwapBuffers();
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            // Send ping every 0.5 seconds
            double currentTime = _pingStopwatch.ElapsedMilliseconds;
            if (currentTime - _lastPingUpdate >= 500 && !_pingSent) // Send ping every 0.5 seconds
            {
                _pingStopwatch.Restart();
                _pingSent = true;
                _lastPingUpdate = currentTime;
                
                Program._udpClient.SendAsync(TcpMessage.FromObject("ping", "ping"));
            }

            if (_gameContext.PlayerObject == null) return;

            var playerObject = _gameContext.MapObjects.FirstOrDefault(e => e.Id == _gameContext.PlayerObject.RenderObjectId);

            if (playerObject == null) return;

            _cameraContext.UpdateCameraPosition(playerObject.Position);

            if (KeyboardState.IsKeyDown(Keys.W) || KeyboardState.IsKeyDown(Keys.Up))
            {
                SendPlayerEvent(PlayerEvents.Jump);
            }

            if (KeyboardState.IsKeyDown(Keys.A) || KeyboardState.IsKeyDown(Keys.Left))
            {
                SendPlayerEvent(PlayerEvents.Left);
            }

            if (KeyboardState.IsKeyDown(Keys.D) || KeyboardState.IsKeyDown(Keys.Right))
            {
                SendPlayerEvent(PlayerEvents.Right);
            }

            if (KeyboardState.IsKeyDown(Keys.F5))
            {
                SendPlayerEvent(PlayerEvents.Reset);
            }
        }
        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (_gameContext.PlayerObject == null) return;

            if (e.Key == Keys.A || e.Key == Keys.Left || e.Key == Keys.D || e.Key == Keys.Right)
            {
                var movementEvent = new HandleMoveEvent()
                {
                    Event = PlayerEvents.Stop,
                    PlayerId = _gameContext.PlayerObject.Id
                };

                Program._udpClient.SendAsync(TcpMessage.FromObject("movement", movementEvent));
            }
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButton.Left && !_gameContext.IsDrawingPlatform)
            {
                var mousePos = new Vector_2(MouseState.X, MouseState.Y);
                var screenSize = new Vector_2(_cameraContext.Width, _cameraContext.Height);
                var normalizedPos = new Vector_2(
                    (mousePos.X / screenSize.X) * 2 - 1,
                    1 - (mousePos.Y / screenSize.Y) * 2
                );
                var aspectRatio = _cameraContext.GetAspectRatio();
                
                _gameContext.PlataformToAddedPosition = new Vector_2(
                    normalizedPos.X * aspectRatio + _cameraContext.CameraOffset.X,
                    normalizedPos.Y + _cameraContext.CameraOffset.Y
                );
                _gameContext.IsDrawingPlatform = true;
                _gameContext.TempPlatform = new RenderObject(_polygonService.CreateRectangle(), PrimitiveType.Triangles) {
                    Position = _gameContext.PlataformToAddedPosition,
                    Static = true,
                    Size = new Vector_2(),
                    Color = new Color_4(0.5f, 0.5f, 0.5f, 0.5f)
                };

            }
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButton.Left && _gameContext.IsDrawingPlatform)
            {
                Vector_2 currentWorldPos = _cameraContext.ScreenToWorld(new Vector_2(MouseState.X, MouseState.Y));

                _gameContext.TempPlatform.CalculatePlatformGeometry(_gameContext.PlataformToAddedPosition, currentWorldPos);

                Program._udpClient.SendAsync(TcpMessage.FromObject("createPlatform", _gameContext.TempPlatform));

                _gameContext.IsDrawingPlatform = false;
                _gameContext.TempPlatform = null;
            }
        }
        private void SendPlayerEvent(PlayerEvents playerEvent)
        {
            var movementEvent = new HandleMoveEvent()
            {
                Event = playerEvent,
                PlayerId = _gameContext.PlayerObject.Id
            };

            Program._udpClient.SendAsync(TcpMessage.FromObject("movement", movementEvent));
        }

        private void DisplayPerformanceInfo()
        {
            Console.SetCursorPosition(0, 0);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"FPS: {_currentFps:F1} | Ping: {_currentPing:F0}ms | Server TPS: {_serverTps:F1}    ");
            Console.ResetColor();
        }
    }
}
