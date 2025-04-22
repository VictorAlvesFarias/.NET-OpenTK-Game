using System.Text;
using System.Text.Json;

namespace Kingdom_of_Creation.Comunication
{
    public static class UdpHelpers
    {
        public static byte[] ToByte<T>(this T item)
        {
            var json = JsonSerializer.Serialize(item);
            var response = Encoding.UTF8.GetBytes(json);

            return response;
        }

        public static T GetData<T>(this byte[] item)
        {
            var json = Encoding.UTF8.GetString(item);
            var response = JsonSerializer.Deserialize<T>(json);

            return response;
        }
    }
}
