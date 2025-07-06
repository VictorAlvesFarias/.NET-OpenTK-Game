using Client.Services.Renders;
using Client.Services.Renders.Implements;
using Kingdom_of_Creation.Entities.Implements;
using Kingdom_of_Creation.Enums;
using Kingdom_of_Creation.Services.RenderObjectService.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Services.Renders.Factories
{
    public class RenderServiceFactory 
    {
        private RectangleRenderService _rectangleRenderer { get; init; }
        private TriangleRenderService _triangleRenderer { get; init; }
        private CircleRenderService _circleRenderer { get; init; }

        public RenderServiceFactory()
        {
            _rectangleRenderer = new RectangleRenderService(
                new RectangleService()
            );
            _triangleRenderer = new TriangleRenderService(
                new TriangleService()
            );
            _circleRenderer = new CircleRenderService(
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
