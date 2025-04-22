using Kingdom_of_Creation;
using System;
using System.Text;

public static class Program
{
    private static UdpServer _udpServer = new UdpServer(11000);
    private static UdpListenerClient _udpClient = new UdpListenerClient(11001, "127.0.0.1", 11000);

    public static void Main()
    {
        _udpServer.Start();

        _udpServer.AddSubscription(data =>
        {
            var message = Encoding.UTF8.GetString(data);
            Console.WriteLine($"Server received: {message}");
        });

        _udpClient.StartListening();
        _udpClient.AddSubscription(data =>
        {
            Console.WriteLine($"Client received: {Encoding.UTF8.GetString(data)}");
        });

        _udpClient.SendAsync(Encoding.UTF8.GetBytes("Test message from client")).Wait();

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();

        _udpClient.Dispose();
        _udpServer.Dispose();
    }
}