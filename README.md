# .NET-OpenTK-Game

## Overview

.NET-OpenTK-Game is a study project focused on developing fundamental game programming concepts, including real-time TCP communication, simple physics, rendering with OpenTK, and client-server architecture. The project demonstrates how to implement a basic multiplayer system with state synchronization, collision detection, and efficient 3D object rendering.

## Table of Contents

- [Overview](#overview)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
  - [Prerequisites](#prerequisites)
  - [Installation](#installation)
  - [Usage](#usage)
- [Main Features](#main-features)
  - [TCP Communication System](#tcp-communication-system)
  - [RenderObject and Property Change Strategy](#renderobject-and-property-change-strategy)

## Project Structure

```
.NET-OpenTK-Game/
├── Client/
│   ├── Assets/Shaders/
│   ├── Context/
│   ├── Extensions/
│   ├── Services/
│   ├── Game.cs
│   └── Program.cs
├── Core/
│   ├── Comunication/
│   │   ├── TcpServer.cs
│   │   ├── TcpClient.cs
│   │   └── TcpMessage.cs
│   ├── Context/
│   ├── Definitions/
│   ├── Dtos/
│   ├── Entities/
│   │   └── Implements/
│   │       ├── Player.cs
│   │       └── RenderObject.cs
│   ├── Enums/
│   ├── Extensions/
│   ├── Physics/
│   │   └── Physics.cs
│   └── Services/
├── Server/
│   ├── Context/
│   ├── Services/
│   ├── Program.cs
│   └── Server.cs
└── Test/
```

## Getting Started

### Prerequisites

Before getting started with .NET-OpenTK-Game, make sure your runtime environment meets the following requirements:

- **.NET 8.0 SDK:** https://dotnet.microsoft.com/download/dotnet/8.0
- **OpenTK 4.x:** https://opentk.net/

### Installation

Install .NET-OpenTK-Game using one of the following methods:

**Build from source:**

Clone the repository:
```sh
git clone <repository-url>
cd .NET-OpenTK-Game
```

### Run the application

**Start the Server:**
```sh
cd Server
dotnet run
```

**Start the Client:**
```sh
cd Client
dotnet run
```

## Main Features

### TCP Communication System

The TCP communication system is the heart of the multiplayer architecture, implemented through three main classes:

#### TcpServer

The `TcpServer` class manages connections from multiple clients and implements a handler system based on message types:

```csharp
public class TcpServer
{
    private readonly TcpListener _listener;
    
    //...

    private readonly ConcurrentDictionary<System.Net.Sockets.TcpClient, NetworkStream> _clients = new();
    private readonly Dictionary<string, List<Action<object, System.Net.Sockets.TcpClient>>> _handlers = new();

    //...

    public void On<T>(string type, Action<T, System.Net.Sockets.TcpClient> handler)
    {
        if (!_handlers.ContainsKey(type))
            _handlers[type] = new();

        _handlers[type].Add((data, client) =>
        {
            var typed = JsonSerializer.Deserialize<T>(data.ToString());
            handler(typed, client);
        });
    }

    //...
}
```

#### TcpMessage

A classe `TcpMessage` implementa serialização/deserialização JSON com prefixo de comprimento para comunicação confiável:

```csharp
public class TcpMessage
{
    public string Type { get; set; }
    public string DataJson { get; set; }

    //...

    public T GetData<T>() => JsonSerializer.Deserialize<T>(DataJson);

    public byte[] ToBytes()
    {
        var json = JsonSerializer.Serialize(this);
        var bytes = Encoding.UTF8.GetBytes(json);
        var lengthPrefix = BitConverter.GetBytes(bytes.Length);
        return lengthPrefix.Concat(bytes).ToArray();
    }

    public static async Task<TcpMessage> FromStreamAsync(NetworkStream stream)
    {
        var lengthBuffer = new byte[4];
        await stream.ReadAsync(lengthBuffer, 0, 4);
        int length = BitConverter.ToInt32(lengthBuffer);

        var buffer = new byte[length];
        int read = 0;
        while (read < length)
        {
            read += await stream.ReadAsync(buffer, read, length - read);
        }

        var json = Encoding.UTF8.GetString(buffer);
        return JsonSerializer.Deserialize<TcpMessage>(json);
    }
}
```

**Communication Strategy:**
- **Length Prefixing**: Each message is preceded by 4 bytes indicating its length
- **JSON Serialization**: Uses System.Text.Json for efficient serialization
- **Type-based Routing**: Messages are routed based on the `Type` field

#### TcpClient

The TCP client implements automatic reconnection and a handler system similar to the server:

```csharp
public class TcpClient
{
    private readonly TcpClient _client = new();

    //...

    private readonly Dictionary<string, List<Action<object>>> _handlers;

    //...

    public void On<T>(string type, Action<T> handler)
    {
        if (!_handlers.ContainsKey(type))
            _handlers[type] = new();

        _handlers[type].Add(data =>
        {
            var typed = JsonSerializer.Deserialize<T>(data.ToString());
            handler(typed);
        });
    }

    //...
}
```

**Key Features:**
- **ConcurrentDictionary**: Manages multiple simultaneous connections in a thread-safe way
- **Handler System**: Allows registering callbacks for different message types
- **Broadcast**: Sends messages to all connected clients
- **Stream Management**: Automatically manages network streams


#### Practical Communication Example

**Sending a message from Client to Server:**
```csharp
// Client sends data to the server
var userData = new UserData()
{
    Name = "John Smith",
    Age = 25,
    Email = "john@email.com"
};

await tcpClient.SendAsync(TcpMessage.FromObject("userRegistration", userData));
```

**Server receives and processes the message:**
```csharp
// Server registers handler for user registration
server.On<UserData>("userRegistration", async (data, client) =>
{
    // Process received data
    var userId = await ProcessUserRegistration(data);
    
    // Respond to the specific client
    var response = new RegistrationResponse() { UserId = userId, Success = true };
    await server.SendAsync(TcpMessage.FromObject("registrationResponse", response), client);
    
    // Notify other clients about the new user
    await server.BroadcastAsync(TcpMessage.FromObject("userJoined", data));
});
```

**Client receives responses:**
```csharp
// Client registers handler for registration response
tcpClient.On<RegistrationResponse>("registrationResponse", (data) =>
{
    if (data.Success)
        Console.WriteLine($"User registered with ID: {data.UserId}");
    else
        Console.WriteLine("Registration failed");
});

// Client registers handler for notifications about other users
tcpClient.On<UserData>("userJoined", (data) =>
{
    Console.WriteLine($"New user connected: {data.Name}");
});
```


### RenderObject and Property Change Strategy

The `RenderObject` class is the core of the rendering and synchronization system, implementing a sophisticated property change detection strategy to optimize network communication:

#### RenderObject Class

```csharp
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

    //...
    
    public float[] Vertices { get; init; } = new float[0];
    
    //... 
    
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
}
```

**Key Features:**
- **Flexible Geometry**: Accepts vertex arrays for different shapes
- **Renderable Properties**: Position, size, color, velocity, and max speed
- **Unique Identification**: Each object has a unique GUID
- **Static/Dynamic State**: Objects can be static or dynamic
- **Smart Serialization**: Control properties are ignored during serialization

#### Event and Change Control
Example:

```csharp
[JsonIgnore]
public List<string> ObjectChanged { get; set; }
```

```csharp
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
```

```csharp
public virtual async Task OnPropertyChanged()
{
    foreach (var handler in OnPropertyChangeSubscriptions)
        await handler(this);
    
    ObjectChanged.RemoveAll(e=>true);
}
```

**Change Detection System:**
- **Property Change Tracking**: Each property automatically marks when it has changed
- **Subscription System**: Objects can subscribe to change notifications
- **Selective Broadcasting**: Only changed properties are sent over the network
- **Internal Updates**: Internal methods allow updates without marking as changed

#### Building the Rendering Structure

```csharp
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
```

```csharp
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
```

**Rendering Strategy:**

- **Vertex Transformation**: Converts local coordinates to world coordinates
- **Scalability**: Applies size to vertices for resizing
- **Flexible Positioning**: Allows rendering at specific positions
- **State Synchronization**: UpdateFrom method synchronizes objects between client and server

---

### Server Change Detection and Sync Example

The server checks for changes in the `RenderObject` and, if any property was changed, broadcasts the update to all clients:

```csharp
private async Task Process(float deltaTime)
{
    float gravity = -9.8f;

    foreach (var item in _gameContext.MapObjects.Where(e => e.Static == false).ToList()) 
    {
        _physics.ResolveColision(item, deltaTime, _gameContext.MapObjects.ToList(), gravity);

        if (item.ObjectChanged.Count > 0)
        {
            await item.OnPropertyChanged(); // Triggers subscriptions, e.g., broadcast to clients
        }
    }
}
```

This ensures only changed objects are sent, optimizing network usage.


---

### Example: Adding a Subscription to RenderObject

Subscriptions allow you to react to property changes in a `RenderObject`. For example, the server can add a subscription to broadcast updates to all clients whenever an object changes:

```csharp
// Add a subscription to broadcast changes
renderObject.OnPropertyChangeSubscriptions.Add(async obj =>
{
    // Broadcast the updated object to all clients
    await Program.UdpServer.BroadcastAsync(TcpMessage.FromObject("updateObject", obj));
});
```

Whenever a property changes and `OnPropertyChanged()` is called, all subscriptions are triggered. In this example, the updated object is sent to all connected clients, ensuring real-time synchronization.

### RenderService Class

The `RenderService` is responsible for drawing objects on the client using OpenTK. It manages OpenGL buffers and draws objects with the specified color and primitive type.

```csharp
public class RenderService 
{
    private int _vertexBufferObject { get; set; }
    private int _vertexArrayObject { get; set; }
    public RenderService()
    {
        _vertexBufferObject = GL.GenBuffer();
        _vertexArrayObject = GL.GenVertexArray();
    }

    public virtual void Draw(float[] vertices, Color_4 color, PrimitiveType primitive)
    {
        UpdateBuffers(vertices);
        Program.GetShader().SetColor4("objectColor", color);
        GL.BindVertexArray(_vertexArrayObject);
        GL.DrawArrays(primitive, 0, vertices.Length / 3);
    }
    public virtual void UpdateBuffers(float[] vertices)
    {
        GL.BindVertexArray(_vertexArrayObject);
        GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
        GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.DynamicDraw);

        GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
        GL.EnableVertexAttribArray(0);
    }
}
```

**How it works:**
- Updates OpenGL buffers with the object's vertices
- Sets the color uniform for the shader
- Draws the object using the specified primitive (e.g., triangles)

---

### Physics Class (SAT Collision)

The `Physics` class is responsible for movement, collision detection, and response. It uses the Separating Axis Theorem (SAT) algorithm to check for collisions between polygons and resolve their movements.

**SAT (Separating Axis Theorem):**
- SAT is a robust algorithm for detecting collisions between convex shapes.
- It works by projecting the shapes onto possible axes and checking for overlaps.
- If there is no overlap on any axis, the shapes do not collide.

**Usage in Physics class:**
- Resolves movement and collision for all dynamic objects
- Applies gravity and velocity
- Triggers collision events (e.g., player landing on a platform)

---

## Deployment

### Production Configuration

1. Update connection strings for production database (if applicable)
2. Configure proper security keys or tokens if needed
3. Set up HTTPS certificates (for web APIs)
4. Configure logging for production environment
5. Set up proper CORS policies (for web APIs)

### Docker Support

The project can be containerized using Docker:

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["Server/Server.csproj", "Server/"]
COPY ["Client/Client.csproj", "Client/"]
COPY ["Core/Core.csproj", "Core/"]
RUN dotnet restore "Server/Server.csproj"
COPY . .
WORKDIR "/src/Server"
RUN dotnet build "Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Server.dll"]
```

## License

This project is licensed under the MIT License - see the LICENSE file for details.