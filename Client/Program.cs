using Client;
using Kingdom_of_Creation.Comunication;
using OpenTK.Compute.OpenCL;

public static class Program
{
    public static Shader ShaderObject = new Shader("./Assets/Shaders/shader.vert", "./Assets/Shaders/shader.frag");
    public static TcpClient _udpClient = new TcpClient();

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
