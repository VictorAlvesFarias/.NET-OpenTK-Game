using Kingdom_of_Creation.Context.Camera.Abstract;
using Kingdom_of_Creation.Dtos;

namespace Server.Context.Camera.Implements
{
    public class CameraContext : CameraContextAbstract
    {
        public CameraContext(float width, float height, Vector_2 cameraOffset, float cameraFollowThreshold, bool windowActive) : base(width, height, cameraOffset, cameraFollowThreshold, windowActive)
        {
        }
    }
}
