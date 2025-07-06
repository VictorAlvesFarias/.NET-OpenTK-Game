using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Entities.Implements;
using Kingdom_of_Creation.Services.EntityGeometry;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;

namespace Kingdom_of_Creation.Services.RenderObjectService.Implements
{
    public class CircleService : IRenderObjectService
    {
        private int _segments { get; init; }
        public CircleService(int segments = 32)
        {
            _segments = segments;
        }

        public float[] GetVertices(RenderObject renderObject)
        {
            float cx = renderObject.Position.X + renderObject.Size.X / 2f;
            float cy = renderObject.Position.Y + renderObject.Size.Y / 2f;
            float rx = renderObject.Size.X / 2f;
            float ry = renderObject.Size.Y / 2f;

            var vertices = new List<float>();

            // Centro do círculo
            vertices.Add(cx);
            vertices.Add(cy);
            vertices.Add(0f);

            for (int i = 0; i <= _segments; i++)
            {
                float theta = (float)(2.0 * Math.PI * i / _segments);
                float x = cx + rx * MathF.Cos(theta);
                float y = cy + ry * MathF.Sin(theta);

                vertices.Add(x);
                vertices.Add(y);
                vertices.Add(0f);
            }

            return vertices.ToArray();
        }
        public PrimitiveType GetPrimitiveType()
        {
            return PrimitiveType.TriangleFan;
        }
        public List<Vector_2> GetVerticesList(RenderObject renderObject)
        {
            float cx = renderObject.Position.X + renderObject.Size.X / 2f;
            float cy = renderObject.Position.Y + renderObject.Size.Y / 2f;
            float rx = renderObject.Size.X / 2f;
            float ry = renderObject.Size.Y / 2f;

            var vertices = new List<Vector_2>
            {
                new Vector_2(cx, cy)
            };

            for (int i = 0; i <= _segments; i++)
            {
                float theta = (float)(2.0 * Math.PI * i / _segments);
                float x = cx + rx * MathF.Cos(theta);
                float y = cy + ry * MathF.Sin(theta);
                vertices.Add(new Vector_2(x, y));
            }

            return vertices;
        }
    }
}
