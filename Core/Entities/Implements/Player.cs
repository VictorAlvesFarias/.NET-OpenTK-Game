using Kingdom_of_Creation.Definitions;
using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Enums;
using Kingdom_of_Creation.Services.PolygonService.Implements;
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
    }
}
