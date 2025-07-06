using Kingdom_of_Creation.Comunication;

namespace Server
{
    public static class Program
    {
        public static bool IsRunning = false;
        public static readonly TcpServer UdpServer = new(25565);

        public static async Task Main(string[] args)
        {
            var server = new ServerApplication();
            await server.InitializeAsync();
        }
    }
}
