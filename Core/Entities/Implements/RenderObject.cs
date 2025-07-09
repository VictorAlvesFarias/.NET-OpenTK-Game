using Kingdom_of_Creation.Definitions;
using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Enums;
using OpenTK.Graphics.OpenGL4;
using System.Text.Json.Serialization;

namespace Kingdom_of_Creation.Entities.Implements
{
    public class RenderObject
    {
        public RenderObject(float[] vertices, PrimitiveType primitiveType)
        {
            Vertices = vertices;
            PrimitiveType = primitiveType;
        }

        public PrimitiveType PrimitiveType { get; set; } 
        public Color_4 Color { get; set; } = ColorDefinitions.Gray;
        public Vector_2 Position { get; set; } = new Vector_2();
        public Vector_2 Size { get; set; } = new Vector_2();
        public Vector_2 Velocity { get; set; } = new Vector_2();
        public Vector_2 Speed { get; set; } = new Vector_2();
        public float[] Vertices { get; init; } = new float[0];
        public bool Static { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid();

        public float[] GetTransformedVertices()
        {
            var transformed = new float[Vertices.Length];
            for (int i = 0; i < Vertices.Length; i += 3)
            {
                float localX = Vertices[i];
                float localY = Vertices[i + 1];
                float localZ = Vertices[i + 2];

                float worldX = Position.X + localX * Size.X;
                float worldY = Position.Y + localY * Size.Y;
                float worldZ = localZ;

                transformed[i] = worldX;
                transformed[i + 1] = worldY;
                transformed[i + 2] = worldZ;
            }

            return transformed;
        }
        public List<Vector_2> GetVerticesList()
        {
            var list = new List<Vector_2>();
            for (int i = 0; i < Vertices.Length; i += 3)
            {
                float localX = Vertices[i];
                float localY =  Vertices[i + 1];
                float worldX = Position.X + localX * Size.X;
                float worldY = Position.Y + localY * Size.Y;
                list.Add(new Vector_2(worldX, worldY));
            }
            return list;
        }
    }
}