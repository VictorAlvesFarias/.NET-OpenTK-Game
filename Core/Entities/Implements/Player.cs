using Kingdom_of_Creation.Definitions;
using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Enums;
using Kingdom_of_Creation.Services.PolygonService.Implements;
using System.Numerics;
using System.Text.Json.Serialization;

namespace Kingdom_of_Creation.Entities.Implements
{
    public class Player
    {
        [JsonIgnore]
        public List<Func<Player, Task>> OnPropertyChangeSubscriptions { get; set; }
        
        private bool _isGrounded;
        private Guid _id;
        private Guid _renderObjectId;
        
        public List<string> ObjectChanged { get; set; }
        
        public bool IsGrounded 
        { 
            get => _isGrounded;
            set
            {
                if (_isGrounded != value)
                {
                    _isGrounded = value;
                    if (!ObjectChanged.Contains(nameof(IsGrounded)))
                        ObjectChanged.Add(nameof(IsGrounded));
                }
            }
        }
        
        public Guid Id 
        { 
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    if (!ObjectChanged.Contains(nameof(Id)))
                        ObjectChanged.Add(nameof(Id));
                }
            }
        }
        
        public Guid RenderObjectId 
        { 
            get => _renderObjectId;
            set
            {
                if (_renderObjectId != value)
                {
                    _renderObjectId = value;
                    if (!ObjectChanged.Contains(nameof(RenderObjectId)))
                        ObjectChanged.Add(nameof(RenderObjectId));
                }
            }
        }

        public Player()
        {
            ObjectChanged = new List<string>();
            OnPropertyChangeSubscriptions = new List<Func<Player, Task>>();
            Id = Guid.NewGuid();
        }

        public void UpdateFrom(Player player)
        {
            IsGrounded = player.IsGrounded;
            Id = player.Id;
            RenderObjectId = player.RenderObjectId;
        }
        
        public virtual async Task OnPropertyChanged()
        {
            foreach (var handler in OnPropertyChangeSubscriptions)
                await handler(this);
        }
    }
}
