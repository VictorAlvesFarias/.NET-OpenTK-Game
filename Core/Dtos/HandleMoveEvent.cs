using Kingdom_of_Creation.Enums;

namespace Kingdom_of_Creation.Dtos
{
    public class HandleMoveEvent
    {
        public Guid PlayerId { get; set; }
        public PlayerEvents Event { get; set; }
    }
}
