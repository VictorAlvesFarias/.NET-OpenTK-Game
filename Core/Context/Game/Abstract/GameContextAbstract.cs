using Kingdom_of_Creation.Entities.Implements;

namespace Kingdom_of_Creation.Context.Game.Abstract
{
    public abstract class GameContextAbstract
    {
        public List<RenderObject> MapObjects { get; set; } 
        public List<Player> ConnectedPlayers { get; set; }

        public GameContextAbstract()
        {
            MapObjects = new List<RenderObject>();
            ConnectedPlayers = new List<Player>();
        }
    }
}
