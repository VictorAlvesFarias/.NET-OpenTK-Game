using Kingdom_of_Creation.Context.Camera.Abstract;
using Kingdom_of_Creation.Dtos;
using OpenTK.Mathematics;
namespace Client.Context.Camera.Implements
{
    public class CameraContext : CameraContextAbstract
    {
        public CameraContext(float width, float height, Vector_2 cameraOffset, float cameraFollowThreshold, bool windowActive) : base(width, height, cameraOffset, cameraFollowThreshold, windowActive)
        {
        }
        public void UpdateWindowSize(float width, float height)
        {
            Width = width;
            Height = height;

            UpdateProjectionMatrix();
        }
        public void UpdateProjectionMatrix()
        {
            float aspectRatio = GetAspectRatio();

            Projection = Matrix4.CreateOrthographicOffCenter(
                -aspectRatio + CameraOffset.X,
                aspectRatio + CameraOffset.X,
                -1f + CameraOffset.Y,
                1f + CameraOffset.Y,
            -1f,
                1f
            );

            Program.GetShader().Use();
            Program.GetShader().SetMatrix4("projection", Projection);
        }
        public Vector_2 ScreenToWorld(Vector_2 screenPos)
        {
            var aspectRatio = GetAspectRatio();
            Vector_2 normalizedPos = new Vector_2(
                (screenPos.X / Width) * 2 - 1,
                1 - (screenPos.Y / Height) * 2
            );

            return new Vector_2(
                normalizedPos.X * aspectRatio + CameraOffset.X,
                normalizedPos.Y + CameraOffset.Y
            );
        }
        public Vector_2 WorldToScreen(Vector_2 worldPos)
        {
            float aspectRatio = (float)Width / Height;
            return new Vector_2(
                (worldPos.X - CameraOffset.X + aspectRatio) * Width / (2 * aspectRatio),
                (worldPos.Y - CameraOffset.Y + 1) * Height / 2
            );
        }
        public void UpdateCameraPosition(Vector_2 position)
        {
            var aspectRatio = GetAspectRatio();
            Vector_2 playerScreenPos = WorldToScreen(position);

            float leftThreshold = Width * CameraFollowThreshold;
            float rightThreshold = Width * (1 - CameraFollowThreshold);

            float bottomThreshold = Height * CameraFollowThreshold;
            float topThreshold = Height * (1 - CameraFollowThreshold);

            var cameraOffset = CameraOffset;

            if (playerScreenPos.X < leftThreshold)
            {
                cameraOffset.X -= (leftThreshold - playerScreenPos.X) / Width * aspectRatio * 2;
            }
            else if (playerScreenPos.X > rightThreshold)
            {
                cameraOffset.X += (playerScreenPos.X - rightThreshold) / Width * aspectRatio * 2;
            }

            if (playerScreenPos.Y < bottomThreshold)
            {
                cameraOffset.Y -= (bottomThreshold - playerScreenPos.Y) / Height * 2;
            }
            else if (playerScreenPos.Y > topThreshold)
            {
                cameraOffset.Y += (playerScreenPos.Y - topThreshold) / Height * 2;
            }

            CameraOffset = cameraOffset;

            UpdateProjectionMatrix();
        }
    }
}
