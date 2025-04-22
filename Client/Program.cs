using Client;
using Kingdom_of_Creation.Comunication;

public static class Program
{
    public static Shader ShaderObject = new Shader("./Assets/Shaders/shader.vert", "./Assets/Shaders/shader.frag");
    public static UdpServerClient _udpClient = new UdpServerClient("127.0.0.1", 25565 );

    public static void Main()
    {
        using (Game game = new Game(800, 600, "OpenTK Platformer"))
        {
            game.Run();
        }
    }
    public static Shader GetShader()
    {
        return ShaderObject;
    }
}
