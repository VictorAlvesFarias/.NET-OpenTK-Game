using Kingdom_of_Creation.Definitions;
using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Enums;
using OpenTK.Graphics.OpenGL4;
using System.Text.Json.Serialization;

namespace Kingdom_of_Creation.Entities.Implements
{
    public class RenderObject
    {
        public EntityShape EntityShape { get; set; } = EntityShape.Rectangle;
        public Color_4 Color { get; set; } = ColorDefinitions.Gray;
        public Vector_2 Position { get; set; } = new Vector_2();
        public Vector_2 Size { get; set; } = new Vector_2();
        public Vector_2 Velocity { get; set; } = new Vector_2();
        public Vector_2 Speed { get; set; } = new Vector_2();
        public bool Static { get; set; }
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}