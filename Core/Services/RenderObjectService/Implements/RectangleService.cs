using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Entities.Implements;
using Kingdom_of_Creation.Services.EntityGeometry;
using Kingdom_of_Creation.Services.RenderObjectService.Abstract;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;

namespace Kingdom_of_Creation.Services.RenderObjectService.Implements
{
    public class RectangleService : RenderObjectServiceAbstract, IRenderObjectService
    {
        public float[] GetVertices(RenderObject renderObject)
        {
            float x = renderObject.Position.X;
            float y = renderObject.Position.Y;
            float w = renderObject.Size.X;
            float h = renderObject.Size.Y;

            return new float[]
            {
            x,     y,     0.0f,
            x + w, y,     0.0f,
            x,     y + h, 0.0f,

            x,     y + h, 0.0f,
            x + w, y,     0.0f,
            x + w, y + h, 0.0f
            };
        }
        public PrimitiveType GetPrimitiveType()
        {
            return PrimitiveType.Triangles;
        }
        public List<Vector_2> GetVerticesList(RenderObject renderObject)
        {
            return new List<Vector_2>()
            {
                renderObject.Position,
                new Vector_2(renderObject.Position.X + renderObject.Size.X, renderObject.Position.Y),
                new Vector_2(renderObject.Position.X + renderObject.Size.X, renderObject.Position.Y + renderObject.Size.Y),
                new Vector_2(renderObject.Position.X, renderObject.Position.Y + renderObject.Size.Y)
            };
        }
    }
}
