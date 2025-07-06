using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Entities.Implements;
using Kingdom_of_Creation.Services.EntityGeometry;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;

namespace Kingdom_of_Creation.Services.RenderObjectService.Implements
{
    public class TriangleService : IRenderObjectService
    {
        public float[] GetVertices(RenderObject renderObject)
        {
            float x = renderObject.Position.X;
            float y = renderObject.Position.Y;
            float w = renderObject.Size.X;
            float h = renderObject.Size.Y;

            return new float[]
            {
                x + w / 2f, y + h, 0.0f, // topo
                x,         y,     0.0f, // base esquerda
                x + w,     y,     0.0f  // base direita
            };
        }
        public PrimitiveType GetPrimitiveType()
        {
            return PrimitiveType.Triangles;
        }
        public List<Vector_2> GetVerticesList(RenderObject renderObject)
        {
            float x = renderObject.Position.X;
            float y = renderObject.Position.Y;
            float w = renderObject.Size.X;
            float h = renderObject.Size.Y;

            return new List<Vector_2>
            {
                new Vector_2(x + w / 2f, y + h), // topo
                new Vector_2(x,         y),     // base esquerda
                new Vector_2(x + w,     y)      // base direita
            };
        }
    }
}
