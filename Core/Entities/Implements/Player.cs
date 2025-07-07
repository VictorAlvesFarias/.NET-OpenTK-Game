using Kingdom_of_Creation.Definitions;
using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Enums;
using System.Numerics;

namespace Kingdom_of_Creation.Entities.Implements
{
    public class Player 
    {
        public bool IsGrounded { get; set; }
        public Guid Id { get; set; } 
        public Guid RenderObjectId { get; set; }

        public Player()
        {
            Id = Guid.NewGuid();
        }

        public RenderObject CreateRenderObject() 
        {
            var renderObject = new RenderObject()
            {
                Position = new Vector_2(0, 0),
                Size = new Vector_2(0.2f, 0.2f),
                Speed = new Vector_2(4f, 4f),
                Id = Guid.NewGuid(),
                EntityShape = EntityShape.Traiangle,
                Color = ColorDefinitions.White
            };

            RenderObjectId = renderObject.Id;

            return renderObject;
        }
    }
}
