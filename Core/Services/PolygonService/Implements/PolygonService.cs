using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Entities.Implements;

namespace Kingdom_of_Creation.Services.PolygonService.Implements
{
    public class PolygonService
    {
        public float[] CreateCircle()
        {
            const int segments = 32; // Number of segments for the circle
            float[] vertices = new float[segments * 3]; // Each vertex has 3 components (x, y, z)

            for (int i = 0; i < segments; i++)
            {
                float angle = (float)(i * 2 * Math.PI / segments);
                float x = (float)Math.Cos(angle);
                float y = (float)Math.Sin(angle);
                vertices[i * 3] = x;       // x coordinate
                vertices[i * 3 + 1] = y;   // y coordinate
                vertices[i * 3 + 2] = 0f;   // z coordinate (flat on the XY plane)
            }
            return vertices;
        }
        public float[] CreateRectangle()
        {
            return new float[]
            {
                0f, 0f, 0f,  // bottom-left
                1f, 0f, 0f,  // bottom-right
                0f, 1f, 0f,  // top-left

                0f, 1f, 0f,  // top-left
                1f, 0f, 0f,  // bottom-right
                1f, 1f, 0f   // top-right
            };
        }
        public float[] CreateTriangle()
        {
            return new float[]
            {
                0.5f, 1f, 0f,  // topo (meio superior)
                0f,   0f, 0f,  // base esquerda
                1f,   0f, 0f   // base direita
            };
        }
    }
}
