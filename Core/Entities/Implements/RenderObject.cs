using Kingdom_of_Creation.Definitions;
using Kingdom_of_Creation.Dtos;
using Kingdom_of_Creation.Enums;
using OpenTK.Graphics.OpenGL4;
using System.Text.Json.Serialization;

namespace Kingdom_of_Creation.Entities.Implements
{
    public class RenderObject 
    {
        [JsonIgnore]
        public List<Func<RenderObject, Task>> OnPropertyChangeSubscriptions { get; set; } 
        
        private PrimitiveType _primitiveType;
        private Color_4 _color;
        private Vector_2 _position;
        private Vector_2 _size;
        private Vector_2 _velocity;
        private Vector_2 _speed;
        private bool _static;
        private Guid _id;

        [JsonIgnore]
        public List<string> ObjectChanged { get; set; }
        public PrimitiveType PrimitiveType 
        { 
            get => _primitiveType;
            private set
            {
                if (_primitiveType != value)
                {
                    _primitiveType = value;
                    if (!ObjectChanged.Contains(nameof(PrimitiveType)))
                        ObjectChanged.Add(nameof(PrimitiveType));
                }
            }
        }
        public Color_4 Color 
        { 
            get => _color;
            set
            {
                if (_color != value)
                {
                    _color = value;
                    if (!ObjectChanged.Contains(nameof(Color)))
                        ObjectChanged.Add(nameof(Color));
                }
            }
        }
        public Vector_2 Position 
        { 
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    if (!ObjectChanged.Contains(nameof(Position)))
                        ObjectChanged.Add(nameof(Position));
                }
            }
        }
        public Vector_2 Size 
        { 
            get => _size;
            set
            {
                if (_size != value)
                {
                    _size = value;
                    if (!ObjectChanged.Contains(nameof(Size)))
                        ObjectChanged.Add(nameof(Size));
                }
            }
        }
        public Vector_2 Velocity 
        { 
            get => _velocity;
            set
            {
                if (_velocity != value)
                {
                    _velocity = value;
                    if (!ObjectChanged.Contains(nameof(Velocity)))
                        ObjectChanged.Add(nameof(Velocity));
                }
            }
        }
        public Vector_2 Speed 
        { 
            get => _speed;
            set
            {
                if (_speed != value)
                {
                    _speed = value;
                    if (!ObjectChanged.Contains(nameof(Speed)))
                        ObjectChanged.Add(nameof(Speed));
                }
            }
        }
        public float[] Vertices { get; init; } = new float[0];
        public bool Static 
        { 
            get => _static;
            set
            {
                if (_static != value)
                {
                    _static = value;
                    if (!ObjectChanged.Contains(nameof(Static)))
                        ObjectChanged.Add(nameof(Static));
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

        public RenderObject(float[] vertices, PrimitiveType primitiveType)
        {
            ObjectChanged = new List<string>();
            Vertices = vertices;
            PrimitiveType = primitiveType;
            Color = ColorDefinitions.Gray;
            Position = new Vector_2();
            Size = new Vector_2();
            Velocity = new Vector_2();
            Speed = new Vector_2();
            Id = Guid.NewGuid();
            OnPropertyChangeSubscriptions = new List<Func<RenderObject, Task>>();
        }
    
        public void UpdateFrom(RenderObject renderObject)
        {
            PrimitiveType = renderObject.PrimitiveType;
            Color = renderObject.Color;
            Position = renderObject.Position;
            Size = renderObject.Size;
            Velocity = renderObject.Velocity;
            Speed = renderObject.Speed;
            Static = renderObject.Static;
            Id = renderObject.Id;
        }
        public float[] GetTransformedVertices(Vector_2? position = null)
        {
            if (position is null)
            {
                position = Position;
            }

            var transformed = new float[Vertices.Length];
            for (int i = 0; i < Vertices.Length; i += 3)
            {
                float localX = Vertices[i];
                float localY = Vertices[i + 1];
                float localZ = Vertices[i + 2];

                float worldX = position.Value.X + localX * Size.X;
                float worldY = position.Value.Y + localY * Size.Y;
                float worldZ = localZ;

                transformed[i] = worldX;
                transformed[i + 1] = worldY;
                transformed[i + 2] = worldZ;
            }

            return transformed;
        }
        public List<Vector_2> GetVerticesList(Vector_2? position = null)
        {
            if (position is null)
            {
                position = Position;
            }

            var list = new List<Vector_2>();
            for (int i = 0; i < Vertices.Length; i += 3)
            {
                float localX = Vertices[i];
                float localY =  Vertices[i + 1];
                float worldX = position.Value.X + localX * Size.X;
                float worldY = position.Value.Y + localY * Size.Y;
                list.Add(new Vector_2(worldX, worldY));
            }
            return list;
        }
        public virtual async Task OnPropertyChanged()
        {
            foreach (var handler in OnPropertyChangeSubscriptions)
                await handler(this);

            ObjectChanged.RemoveAll(e=>true);
        }

        // Métodos internos para atualização sem marcar como alterado (usado pela classe Physics)
        internal void UpdatePositionInternal(Vector_2 newPosition)
        {
            _position = newPosition;
        }

        internal void UpdateVelocityInternal(Vector_2 newVelocity)
        {
            _velocity = newVelocity;
        }
    }
}