
using Kingdom_of_Creation.Definitions;
using Kingdom_of_Creation.Dtos;

namespace Kingdom_of_Creation.Entities
{
    public class Platform : Rectangle
    {
        public Platform(Vector_2 position, Vector_2 size) : base(position, size, ColorDefinitions.Gray) { }
        public Platform() { }
    }
}
