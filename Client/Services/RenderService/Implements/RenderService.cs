using Kingdom_of_Creation.Definitions;
using Kingdom_of_Creation.Dtos;
using OpenTK.Graphics.OpenGL4;

namespace Client.Services
{
    public class RenderService 
    {
        private int _vertexBufferObject { get; set; }
        private int _vertexArrayObject { get; set; }
        public RenderService()
        {
            _vertexBufferObject = GL.GenBuffer();
            _vertexArrayObject = GL.GenVertexArray();
        }

        public virtual void Draw(float[] vertices, Color_4 color, PrimitiveType primitive)
        {
            UpdateBuffers(vertices);
            Program.GetShader().SetColor4("objectColor", color ?? ColorDefinitions.White);
            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawArrays(primitive, 0, vertices.Length / 3);
        }
        public virtual void UpdateBuffers(float[] vertices)
        {
            GL.BindVertexArray(_vertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }
    }
}
