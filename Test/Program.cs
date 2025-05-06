

using Kingdom_of_Creation.Comunication;

public class MyEventData
{
    public string Message { get; set; }
}

internal class Program
{
    private static async Task Main(string[] args)
    {
        var server = new TcpServer(5000);

        server.On<MyEventData>("my-event", async (data, client) =>
        {
            Console.WriteLine($"[Servidor] Recebido: {data.Message}");

            var resposta = TcpMessage.FromObject("server-response", new MyEventData
            {
                Message = "Mensagem recebida com sucesso!"
            });

            await server.SendAsync(resposta, client);
        });


        _ = Task.Run(() => server.StartAsync());

        await Task.Delay(500);

        var client = new TcpClient();

        client.On<MyEventData>("server-response", data =>
        {
            Console.WriteLine($"[Cliente] Resposta do servidor: {data.Message}");
        });

        client.Connect("127.0.0.1", 5000);

        var mensagem = TcpMessage.FromObject("my-event", new MyEventData
        {
            Message = "Olá servidor!"
        });

        await client.SendAsync(mensagem);

        Console.ReadLine();
    }
}
