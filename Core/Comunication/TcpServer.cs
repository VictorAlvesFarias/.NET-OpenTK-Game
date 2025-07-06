using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kingdom_of_Creation.Comunication
{

    public class TcpServer
    {
        private readonly TcpListener _listener;
        private readonly CancellationTokenSource _cts = new();
        private readonly ConcurrentDictionary<System.Net.Sockets.TcpClient, NetworkStream> _clients = new();
        private readonly Dictionary<string, List<Action<object, System.Net.Sockets.TcpClient>>> _handlers = new();

        public TcpServer(int port)
        {
            _listener = new TcpListener(IPAddress.Any, port);
        }

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

        public async Task StartAsync()
        {
            _listener.Start();

            while (!_cts.Token.IsCancellationRequested)
            {
                var client = await _listener.AcceptTcpClientAsync();
                var stream = client.GetStream();
                _clients.TryAdd(client, stream);
                _ = HandleClientAsync(client, stream);
            }
        }

        private async Task HandleClientAsync(System.Net.Sockets.TcpClient client, NetworkStream stream)
        {
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    var message = await TcpMessage.FromStreamAsync(stream);
                    if (_handlers.TryGetValue(message.Type, out var handlers))
                    {
                        foreach (var h in handlers)
                            h(message.DataJson, client);
                    }
                }
            }
            catch { }
            finally
            {

                _clients.TryRemove(client, out _);
                client.Close();
            }
        }

        public async Task SendAsync(TcpMessage message, System.Net.Sockets.TcpClient client)
        {
            var stream = _clients[client];
            var bytes = message.ToBytes();
            await stream.WriteAsync(bytes, 0, bytes.Length);
        }

        public async Task BroadcastAsync(TcpMessage message)
        {
            var bytes = message.ToBytes();
            foreach (var stream in _clients.Values)
                await stream.WriteAsync(bytes, 0, bytes.Length);
        }

        public void Stop()
        {
            _cts.Cancel();
            foreach (var client in _clients.Keys)
                client.Close();
            _listener.Stop();
        }
    }
}
