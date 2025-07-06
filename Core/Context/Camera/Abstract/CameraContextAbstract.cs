using Kingdom_of_Creation.Dtos;
using OpenTK.Mathematics;

namespace Kingdom_of_Creation.Context.Camera.Abstract
{
    public abstract class CameraContextAbstract
    {
        public float Width { get; set; }
        public float Height { get; set; }
        public Vector_2 CameraOffset { get; set; }
        public float CameraFollowThreshold { get; set; }
        public bool WindowActive { get; set; }
        public Matrix4 Projection { get; set; }

        public CameraContextAbstract(float width, float height, Vector_2 cameraOffset, float cameraFollowThreshold, bool windowActive)
        {
            Width = width;
            Height = height;
            CameraOffset = cameraOffset;
            CameraFollowThreshold = cameraFollowThreshold;
            WindowActive = windowActive;
        }

        public float GetAspectRatio()
        {
            return Width / Height;
        }
    }
}
