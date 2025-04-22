using System.Net;
using System.Net.Sockets;

namespace Kingdom_of_Creation.Comunication;

public class UdpServerClient : IDisposable
{
    private readonly UdpClient _udpClient;
    private readonly IPEndPoint _serverEndPoint;
    private readonly CancellationTokenSource _cancellation = new();
    private readonly List<Action<UdpMessage>> _subscriptions = new();

    public UdpServerClient(string serverIp, int serverPort)
    {
        _udpClient = new UdpClient(new IPEndPoint(IPAddress.Any, 0));
        _serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
        Task.Run(ReceiveLoop, _cancellation.Token);
    }

    public async Task SendAsync(UdpMessage message)
    {
        var bytes = message.ToByte<UdpMessage>();
        await _udpClient.SendAsync(bytes, bytes.Length, _serverEndPoint);
    }

    public void Subscribe(Action<UdpMessage> callback)
    {
        _subscriptions.Add(callback);
    }

    private async Task ReceiveLoop()
    {
        while (!_cancellation.Token.IsCancellationRequested)
        {
            try
            {
                var result = await _udpClient.ReceiveAsync();
                var message = result.Buffer.GetData<UdpMessage>();

                foreach (var callback in _subscriptions)
                    callback(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Cliente] Erro: {ex.Message}");
            }
        }
    }

    public void Dispose()
    {
        _cancellation.Cancel();
        _udpClient.Close();
        _udpClient.Dispose();
    }
}
