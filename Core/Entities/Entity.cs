
using Kingdom_of_Creation.Dtos;

namespace Kingdom_of_Creation.Entities
{
    public class Entity : Rectangle
    {
        public Vector_2 Velocity { get; set; }
        public float Speed { get; set; }
        public Guid Id { get; set; } 
        public bool IsGrounded { get; set; }
        public float JumpForce { get; set; }

        public Entity(Vector_2 position, Vector_2 size, Color_4 color, float speed,Guid id, float jumpForce) : base(position, size, color)
        {
            Velocity = new();
            Speed = speed;
            JumpForce = jumpForce;
            Id = id;
        }
        public Entity() { }
    }
}
