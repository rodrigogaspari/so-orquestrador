using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace so_orquestrador.Infrastructure.Queue.Util
{
    public static class QueueUtil
    {
        public static string FilaCancelar { get { return "cancelar-fila";} }
        public static string FilaCancelarDlq { get { return "cancelar-dlq"; } }

        public static void ConfigureQueuesProcucer()
        {
            // Cria a factory conforme a instância local (apenas experimentos)
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "admin", // conforme docker-compose
                Password = "admin"
            };

            // Cria a conexão e canal.
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Declara a DLQ
            channel.QueueDeclare(
                queue: FilaCancelarDlq,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            // Configura argumentos para a fila principal
            var args = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", "" }, // usa o default exchange
                { "x-dead-letter-routing-key", FilaCancelarDlq }
            };

            // Declara a fila principal com DLQ configurada
            channel.QueueDeclare(
                queue: FilaCancelar,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: args);

            Console.WriteLine("Fila principal e DLQ configuradas com sucesso!");
        }

        public static void PublicarMensagem(string fila, object mensagem)
        {
            // Cria a factory conforme a instância local (apenas experimentos)
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "admin",
                Password = "admin"
            };

            // Cria a conexão e canal.
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            // Serializa a mensagem 
            var json = JsonSerializer.Serialize(mensagem);
            var body = Encoding.UTF8.GetBytes(json);

            // Publica na fila indicada
            channel.BasicPublish(
                    exchange: "",
                    routingKey: fila,
                    basicProperties: null,
                    body: body);

            Console.WriteLine("Mensagem publicada!");
        }

    }
}

