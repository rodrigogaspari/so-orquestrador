using RabbitMQ.Client;
using so_orquestrador.Infrastructure.Queue.Util;
using so_orquestrador.Infrastructure.Services.Extensions;
using System.Text;
using System.Text.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var services = builder.Services;

        // Add controllers
        services.AddControllers();

        // Add Http Clients to orchetrator
        services.AddHttpClients();

        // Configure Swagger in app services
        services.ConfigureSwaggerAppServices();

        // Configurando as filas da aplicação
        QueueUtil.ConfigureQueuesProcucer();

        var app = builder.Build();

        // Activate Swagger documentation in developmant environment
        app.ActivateSwaggerInDebug(); 
       
        app.MapControllers();

        app.Run();
    }
}