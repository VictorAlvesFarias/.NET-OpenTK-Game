using Kingdom_of_Creation.Dtos;
using OpenTK.Graphics.OpenGL4;

namespace Kingdom_of_Creation.Entities
{
    public abstract class RenderObject
    {
        public int VertexArrayObject { get; set; }
        public int VertexBufferObject { get; set; }
        public Color_4 Color { get; set; }
        public bool Initialized { get; set; }

        public abstract float[] GetVertices();
        public abstract PrimitiveType GetPrimitiveType();
    }
}