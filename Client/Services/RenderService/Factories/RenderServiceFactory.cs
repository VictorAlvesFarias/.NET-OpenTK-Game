using Kingdom_of_Creation.Entities.Implements;
using Kingdom_of_Creation.Enums;
using Kingdom_of_Creation.Services.RenderObjectService.Implements;

namespace Client.Services.Renders.Factories
{
    public class RenderServiceFactory 
    {
        private RenderService _rectangleRenderer { get; init; }
        private RenderService _triangleRenderer { get; init; }
        private RenderService _circleRenderer { get; init; }

        public RenderServiceFactory()
        {
            _rectangleRenderer = new RenderService(
                new RectangleService()
            );
            _triangleRenderer = new RenderService(
                new TriangleService()
            );
            _circleRenderer = new RenderService(
                new CircleService()
            );
        }

        public IRenderService GetRenderService(RenderObject renderObject)
        {
            if (renderObject.EntityShape == EntityShape.Rectangle)
            {
                return _rectangleRenderer;
            }
            if (renderObject.EntityShape == EntityShape.Traiangle)
            {
                return _triangleRenderer;
            }
            if (renderObject.EntityShape == EntityShape.Circle)
            {
                return _circleRenderer;
            }
            else
            {
                throw new NotSupportedException($"RenderObject of type {renderObject.GetType()} is not supported.");
            }
        }
    }
}
