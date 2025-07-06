using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Entities.Implements;
using Kingdom_of_Creation.Enums;
using Kingdom_of_Creation.Services.EntityGeometry;
using Kingdom_of_Creation.Services.RenderObjectService.Implements;
using OpenTK.Graphics.OpenGL4;

namespace Kingdom_of_Creation.Services.RenderObjectService.Factories
{
    public class RenderObjectFactory 
    {
        private CircleService _circleService { get; init; }
        private TriangleService _triangleService { get; init; }
        private RectangleService _rectangleService { get; init; }
        public RenderObjectFactory()
        {
            _circleService = new CircleService();
            _triangleService = new TriangleService();
            _rectangleService = new RectangleService();
        }

        public IRenderObjectService GetRenderService(RenderObject renderObject)
        {
            if (renderObject.EntityShape == EntityShape.Rectangle)
            {
                return _rectangleService;
            }
            if (renderObject.EntityShape == EntityShape.Circle)
            {
                return _circleService;
            }
            if (renderObject.EntityShape == EntityShape.Traiangle)
            {
                return _triangleService;
            }
            else
            {
                throw new NotSupportedException($"RenderObject of type {renderObject.GetType()} is not supported.");
            }
        }
    }
}
