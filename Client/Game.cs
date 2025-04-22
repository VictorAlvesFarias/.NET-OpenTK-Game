using Kingdom_of_Creation.Entities;
using Kingdom_of_Creation.Enums;
using Kingdom_of_Creation;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL4;
using Kingdom_of_Creation.Comunication;
using Kingdom_of_Creation.Dtos;

namespace Client
{
    public class Game : GameWindow
    {
        private Player PlayerObject;
        private List<Rectangle> MapObjects = new();
        private List<Player> ConnectedPlayers = new();
        private Matrix4 Projection;
        private int Width;
        private int Height;
        private Vector_2 _cameraOffset = new();
        private bool WindowActive;
        private float CameraFollowThreshold = 0.3f;
        private Vector_2 PlataformToAddedPosition = new();
        private Rectangle _tempPlatform;
        private bool _isDrawingPlatform = false;

        public Game(int width, int height, string title) : base(GameWindowSettings.Default, new NativeWindowSettings() { Size = (width, height), Title = title })
        {
            base.OnLoad();

            Width = width;
            Height = height;
        }

        protected override void OnFocusedChanged(FocusedChangedEventArgs e)
        {
            base.OnFocusedChanged(e);

            WindowActive = e.IsFocused;
        }
        protected override void OnLoad()
        {
            base.OnLoad();

            float aspectRatio = (float)Width / Height;

            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            Projection = Matrix4.CreateOrthographicOffCenter(
                -aspectRatio,
                aspectRatio,
                -1f,
                1f,
                -1f,
                1f
            );

            Program.GetShader().Use();
            Program.GetShader().SetMatrix4("projection", Projection);

            Program._udpClient.Subscribe((data) =>
            {
                Console.WriteLine($"Received message of type: {data.Type}");

                if (data.Type == "onConnectionOpened")
                {
                    PlayerObject = data.Data.GetData<Player>();

                    Console.WriteLine($"Player initialized with ID: {PlayerObject.Id}");
                }
                else if (data.Type == "updatePlayers")
                {
                    var updatedPlayer = data.Data.GetData<List<Player>>();

                    ConnectedPlayers = updatedPlayer;
                }
                else if (data.Type == "updateMap")
                {
                    MapObjects = data.Data.GetData<List<Rectangle>>();
                }
            });

            var initRequest = new UdpMessage()
            {
                Type = "requestInitPlayer",
                Data = Array.Empty<byte>()
            };

            Program._udpClient.SendAsync(initRequest);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit);
            Program.GetShader().Use();

            foreach (var obj in MapObjects)
            {
                if (obj.Initialized == false)
                {
                    obj.Initialize();
                }

                obj.Draw();
            }

            foreach (var obj in ConnectedPlayers)
            {
                if (obj.Id == PlayerObject.Id)
                {
                    PlayerObject = obj;
                }

                if (obj.Initialized == false)
                {
                    obj.Initialize();
                }

                obj.Draw();
            }

            if (_isDrawingPlatform && _tempPlatform != null)
            {
                Vector_2 currentWorldPos = ScreenToWorld(new Vector_2(MouseState.X, MouseState.Y));
                var (position, size) = CalculatePlatformGeometry(PlataformToAddedPosition, currentWorldPos);

                _tempPlatform.Position = position;
                _tempPlatform.Size = size;
                _tempPlatform.Draw();
            }

            SwapBuffers();
        }
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            if (PlayerObject == null) return;

            UpdateCameraPosition();

            if (KeyboardState.IsKeyDown(Keys.W) || KeyboardState.IsKeyDown(Keys.Up))
            {
                SendMovementEvent(PlayerEvents.Jump);
            }

            if (KeyboardState.IsKeyDown(Keys.A) || KeyboardState.IsKeyDown(Keys.Left))
            {
                SendMovementEvent(PlayerEvents.Left);
            }

            if (KeyboardState.IsKeyDown(Keys.D) || KeyboardState.IsKeyDown(Keys.Right))
            {
                SendMovementEvent(PlayerEvents.Right);
            }
        }
        private void SendMovementEvent(PlayerEvents playerEvent)
        {
            var movementEvent = new HandleMoveEvent()
            {
                Event = playerEvent,
                PlayerId = PlayerObject.Id
            };

            var senderObject = new UdpMessage()
            {
                Data = movementEvent.ToByte(),
                Type = "movement"
            };

            Program._udpClient.SendAsync(senderObject);
        }
        protected override void OnKeyUp(KeyboardKeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (PlayerObject == null) return;

            if (e.Key == Keys.A || e.Key == Keys.Left || e.Key == Keys.D || e.Key == Keys.Right)
            {
                var movementEvent = new HandleMoveEvent()
                {
                    Event = PlayerEvents.Stop,
                    PlayerId = PlayerObject.Id
                };

                var senderObject = new UdpMessage()
                {
                    Data = movementEvent.ToByte(),
                    Type = "movement"

                };

                Program._udpClient.SendAsync(senderObject);
            }
        }
        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);

            if (e.Button == MouseButton.Left && !_isDrawingPlatform)
            {
                // Converte coordenadas de tela para coordenadas do mundo
                Vector_2 mousePos = new Vector_2(MouseState.X, MouseState.Y);
                Vector_2 screenSize = new Vector_2(Width, Height);

                // Converte para coordenadas normalizadas (-1 a 1)
                Vector_2 normalizedPos = new Vector_2(
                    (mousePos.X / screenSize.X) * 2 - 1,
                    1 - (mousePos.Y / screenSize.Y) * 2
                );

                // Ajusta para o sistema de coordenadas do mundo
                float aspectRatio = (float)Width / Height;
                PlataformToAddedPosition = new Vector_2(
                    normalizedPos.X * aspectRatio + _cameraOffset.X,
                    normalizedPos.Y + _cameraOffset.Y
                );

                _isDrawingPlatform = true;
                _tempPlatform = new Rectangle(PlataformToAddedPosition, new Vector_2(), new Color_4(0.5f, 0.5f, 0.5f, 0.5f));
                _tempPlatform.Initialize();

                Console.WriteLine($"Platform start position: {PlataformToAddedPosition}");
            }
        }
        private void UpdateProjectionMatrix()
        {
            float aspectRatio = (float)Width / Height;

            Projection = Matrix4.CreateOrthographicOffCenter(
                -aspectRatio + _cameraOffset.X,
                aspectRatio + _cameraOffset.X,
                -1f + _cameraOffset.Y,
                1f + _cameraOffset.Y,
                -1f,
                1f
            );

            Program.GetShader().Use();
            Program.GetShader().SetMatrix4("projection", Projection);
        }
        private (Vector_2 position, Vector_2 size) CalculatePlatformGeometry(Vector_2 startWorldPos, Vector_2 currentWorldPos)
        {
            // Calcula o tamanho absoluto
            Vector_2 size = new Vector_2(
                Math.Abs(currentWorldPos.X - startWorldPos.X),
                Math.Abs(currentWorldPos.Y - startWorldPos.Y)
            );

            // Aplica tamanho mínimo
            size.X = Math.Max(size.X, 0.1f);
            size.Y = Math.Max(size.Y, 0.1f);

            // Calcula a posição correta (canto inferior esquerdo)
            Vector_2 position = new Vector_2(
                Math.Min(startWorldPos.X, currentWorldPos.X),
                Math.Min(startWorldPos.Y, currentWorldPos.Y)
            );

            return (position, size);
        }
        private Vector_2 ScreenToWorld(Vector_2 screenPos)
        {
            float aspectRatio = (float)Width / Height;
            Vector_2 normalizedPos = new Vector_2(
                (screenPos.X / Width) * 2 - 1,
                1 - (screenPos.Y / Height) * 2
            );

            return new Vector_2(
                normalizedPos.X * aspectRatio + _cameraOffset.X,
                normalizedPos.Y + _cameraOffset.Y
            );
        }
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (e.Button == MouseButton.Left && _isDrawingPlatform)
            {
                Vector_2 currentWorldPos = ScreenToWorld(new Vector_2(MouseState.X, MouseState.Y));
                var (position, size) = CalculatePlatformGeometry(PlataformToAddedPosition, currentWorldPos);

                // Enviar a nova plataforma para o servidor
                var platformData = new { Position = position, Size = size };
                var message = new UdpMessage()
                {
                    Type = "createPlatform",
                    Data = platformData.ToByte()
                };
                Program._udpClient.SendAsync(message);

                _isDrawingPlatform = false;
                _tempPlatform = null;
            }
        }
        private void UpdateCameraPosition()
        {
            if (PlayerObject == null) return;

            float aspectRatio = (float)Width / Height;
            Vector_2 playerScreenPos = WorldToScreen(PlayerObject.Position);

            float leftThreshold = Width * CameraFollowThreshold;
            float rightThreshold = Width * (1 - CameraFollowThreshold);

            float bottomThreshold = Height * CameraFollowThreshold;
            float topThreshold = Height * (1 - CameraFollowThreshold);

            // Acompanhamento horizontal
            if (playerScreenPos.X < leftThreshold)
            {
                _cameraOffset.X -= (leftThreshold - playerScreenPos.X) / Width * aspectRatio * 2;
            }
            else if (playerScreenPos.X > rightThreshold)
            {
                _cameraOffset.X += (playerScreenPos.X - rightThreshold) / Width * aspectRatio * 2;
            }

            // Acompanhamento vertical
            if (playerScreenPos.Y < bottomThreshold)
            {
                _cameraOffset.Y -= (bottomThreshold - playerScreenPos.Y) / Height * 2;
            }
            else if (playerScreenPos.Y > topThreshold)
            {
                _cameraOffset.Y += (playerScreenPos.Y - topThreshold) / Height * 2;
            }

            UpdateProjectionMatrix();
        }
        private Vector_2 WorldToScreen(Vector_2 worldPos)
        {
            float aspectRatio = (float)Width / Height;
            return new Vector_2(
                (worldPos.X - _cameraOffset.X + aspectRatio) * Width / (2 * aspectRatio),
                (worldPos.Y - _cameraOffset.Y + 1) * Height / 2
            );
        }
    }
}
