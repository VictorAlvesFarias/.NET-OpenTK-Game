using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Kingdom_of_Creation.Comunication
{
    public class TcpMessage
    {
        public string Type { get; set; }
        public string DataJson { get; set; }

        public static TcpMessage FromObject(string type, object data)
        {
            return new TcpMessage
            {
                Type = type,
                DataJson = JsonSerializer.Serialize(data)
            };
        }

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

}
