
using Kingdom_of_Creation.Definitions;
using Kingdom_of_Creation.Dtos;

namespace Kingdom_of_Creation.Entities
{
    public class Player : Entity
    {
        public Player(Vector_2 position, Vector_2 size, float jumpForce, float speed, Guid playerId) : base(position, size, ColorDefinitions.White, speed, playerId, jumpForce)
        {
        }
        public Player() { }
    }
}
