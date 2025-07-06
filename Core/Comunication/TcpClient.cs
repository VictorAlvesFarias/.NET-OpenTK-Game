using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kingdom_of_Creation.Comunication
{
    public class TcpClient
    {
        private readonly System.Net.Sockets.TcpClient _client = new();
        private NetworkStream _stream;
        private readonly Dictionary<string, List<Action<object>>> _handlers = new();
        private readonly CancellationTokenSource _cts = new();

        public void Connect(string host, int port)
        {
            _client.Connect(host, port);
            _stream = _client.GetStream();
            _ = ReceiveLoop();
        }

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

        private async Task ReceiveLoop()
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                var message = await TcpMessage.FromStreamAsync(_stream);
                if (_handlers.TryGetValue(message.Type, out var handlers))
                {
                    foreach (var h in handlers)
                        h(message.DataJson);
                }
            }
        }

        public async Task SendAsync(TcpMessage message)
        {
            var bytes = message.ToBytes();
            await _stream.WriteAsync(bytes, 0, bytes.Length);
        }

        public void Disconnect()
        {
            _cts.Cancel();
            _client.Close();
        }
    }
}
