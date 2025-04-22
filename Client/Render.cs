using Kingdom_of_Creation.Entities;
using OpenTK.Graphics.OpenGL4;

namespace Client
{
    public static class Render
    {
        public static void Draw(this RenderObject renderObject)
        {
            if (renderObject == null) return;

            UpdateBuffers(renderObject);
            Program.GetShader().SetColor4("objectColor", renderObject.Color); // Define a cor antes de desenhar
            GL.BindVertexArray(renderObject.VertexArrayObject);
            GL.DrawArrays(renderObject.GetPrimitiveType(), 0, renderObject.GetVertices().Length / 3);
        }
        public static void Initialize(this RenderObject renderObject)
        {
            if (renderObject == null) return;

            renderObject.VertexBufferObject = GL.GenBuffer();
            renderObject.VertexArrayObject = GL.GenVertexArray();
            renderObject.Initialized = true;

            UpdateBuffers(renderObject);
        }
        public static void UpdateBuffers(this RenderObject renderObject)
        {
            if (renderObject == null) return;

            GL.BindVertexArray(renderObject.VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, renderObject.VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, renderObject.GetVertices().Length * sizeof(float), renderObject.GetVertices(), BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }
    }

}
