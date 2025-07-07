using Kingdom_of_Creation.Entities.Implements;

namespace Client.Services.Renders
{
    public interface IRenderService
    {
        void Draw(RenderObject renderObject);
        void UpdateBuffers(RenderObject renderObject);
    }
}
