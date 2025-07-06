using Client.Services.Renders;
using Kingdom_of_Creation.Entities.Implements;
using Kingdom_of_Creation.Services.EntityGeometry;
using OpenTK.Graphics.OpenGL4;

namespace Client.Services.Renders.Implements
{
    public class TriangleRenderService : IRenderService
    {
        private readonly IRenderObjectService _renderObjectService;
        public TriangleRenderService(IRenderObjectService renderObjectService)
        {
            _renderObjectService = renderObjectService;
        }
        public void Draw(RenderObject renderObject)
        {
            if (renderObject == null)
            {
                return;
            }

            UpdateBuffers(renderObject);
            Program.GetShader().SetColor4("objectColor", renderObject.Color);
            GL.BindVertexArray(renderObject.VertexArrayObject);
            GL.DrawArrays(_renderObjectService.GetPrimitiveType(), 0, _renderObjectService.GetVertices(renderObject).Length / 3);
        }

        public void Initialize(RenderObject renderObject)
        {
            if (renderObject == null)
            {
                return;
            }

            renderObject.VertexBufferObject = GL.GenBuffer();
            renderObject.VertexArrayObject = GL.GenVertexArray();
            renderObject.Initialized = true;

            UpdateBuffers(renderObject);
        }

        public void UpdateBuffers(RenderObject renderObject)
        {
            if (renderObject == null)
            {
                return;
            }

            GL.BindVertexArray(renderObject.VertexArrayObject);
            GL.BindBuffer(BufferTarget.ArrayBuffer, renderObject.VertexBufferObject);
            GL.BufferData(BufferTarget.ArrayBuffer, _renderObjectService.GetVertices(renderObject).Length * sizeof(float), _renderObjectService.GetVertices(renderObject), BufferUsageHint.DynamicDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
        }
    }
}
