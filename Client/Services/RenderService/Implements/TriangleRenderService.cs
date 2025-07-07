using Client.Services.Renders;
using Kingdom_of_Creation.Entities.Implements;
using Kingdom_of_Creation.Services.EntityGeometry;
using OpenTK.Graphics.OpenGL4;

namespace Client.Services.Renders.Implements
{
    public class TriangleRenderService : IRenderService
    {
        private readonly IRenderObjectService _renderObjectService;
        private int _vertexBufferObject { get; set; }
        private int _vertexArrayObject { get; set; }
        public TriangleRenderService(IRenderObjectService renderObjectService)
        {
            _renderObjectService = renderObjectService;
            _vertexBufferObject = GL.GenBuffer();
            _vertexArrayObject = GL.GenVertexArray();
        }
        public void Draw(RenderObject renderObject)
        {
            if (renderObject == null)
            {
                return;
            }

            UpdateBuffers(renderObject);
            Program.GetShader().SetColor4("objectColor", renderObject.Color);
            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawArrays(_renderObjectService.GetPrimitiveType(), 0, _renderObjectService.GetVertices(renderObject).Length / 3);
        }
        public void UpdateBuffers(RenderObject renderObject)
        {
            if (renderObject == null)
            {
                return;
            }

            GL.BindVertexArray(_vertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _renderObjectService.GetVertices(renderObject).Length * sizeof(float), _renderObjectService.GetVertices(renderObject), BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }
    }
}
