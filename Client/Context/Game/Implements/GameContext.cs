using Kingdom_of_Creation.Context.Game.Abstract;
using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Entities.Implements;

namespace Client.Context.Camera.Implements
{
    public class GameContext : GameContextAbstract
    {
        public Vector_2 PlataformToAddedPosition { get; set; }
        public RenderObject TempPlatform { get; set; }
        public bool IsDrawingPlatform  { get; set; }
        public Player PlayerObject { get; set; }
    }
}
