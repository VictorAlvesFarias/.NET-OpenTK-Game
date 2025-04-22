using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace Kingdom_of_Creation.Comunication;

public class UdpServer : IDisposable
{
    private readonly UdpClient _udpServer;
    private readonly CancellationTokenSource _cancellation = new();
    public readonly ConcurrentDictionary<string, IPEndPoint> _connectedClients = new();
    private readonly List<Action<UdpMessage, IPEndPoint>> _subscriptions = new();

    public UdpServer(int port)
    {
        _udpServer = new UdpClient(port);
        Task.Run(ReceiveLoop, _cancellation.Token);
    }

    public void Subscribe(Action<UdpMessage, IPEndPoint> callback)
    {
        _subscriptions.Add(callback);
    }

    private async Task ReceiveLoop()
    {
        while (!_cancellation.Token.IsCancellationRequested)
        {
            try
            {
                var result = await _udpServer.ReceiveAsync();
                var message = result.Buffer.GetData<UdpMessage>();

                var key = result.RemoteEndPoint.ToString();
                _connectedClients.TryAdd(key, result.RemoteEndPoint);

                foreach (var callback in _subscriptions)
                    callback(message, result.RemoteEndPoint);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Servidor] Erro: {ex.Message}");
            }
        }
    }

    public async Task SendToAsync(UdpMessage message, IPEndPoint destination)
    {
        var bytes = message.ToByte();
        await _udpServer.SendAsync(bytes, bytes.Length, destination);
    }

    public async Task BroadcastAsync(UdpMessage message)
    {
        var bytes = message.ToByte();
        foreach (var client in _connectedClients.Values)
        {
            await _udpServer.SendAsync(bytes, bytes.Length, client);
        }
    }

    public void Dispose()
    {
        _cancellation.Cancel();
        _udpServer.Close();
        _udpServer.Dispose();
    }
}
