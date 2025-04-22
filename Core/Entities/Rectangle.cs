using Kingdom_of_Creation.Dtos;
using OpenTK.Graphics.OpenGL4;

namespace Kingdom_of_Creation.Entities
{
    public class Rectangle : RenderObject
    {
        public Vector_2 Position { get; set; }
        public Vector_2 Size { get; set; }
        public Rectangle(Vector_2 position, Vector_2 size, Color_4 color)
        {
            Position = position;
            Size = size;
            Color = color;
        }
        public Rectangle() { }

        public override float[] GetVertices()
        {
            float x = Position.X;
            float y = Position.Y;
            float w = Size.X;
            float h = Size.Y;

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

        public override PrimitiveType GetPrimitiveType()
        {
            return PrimitiveType.Triangles;
        }
    }
}
