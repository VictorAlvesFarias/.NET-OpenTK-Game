using Kingdom_of_Creation.Comunication;
using static System.Runtime.InteropServices.JavaScript.JSType;

public static class Program
{
    public static UdpServerClient _udpClient = new UdpServerClient("189.123.70.20", 25565);

    public static void Main()
    {
        try
        {
            _udpClient.Subscribe(data =>
            {
                Console.WriteLine($"Received message of type: {data.Type}");
            });


            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });
            _udpClient.SendAsync(new UdpMessage() { Type = "ping", Data = [] });

            while (true)
            {
                
            }
        }
		catch (Exception)
		{

			throw;
		}
    }
}