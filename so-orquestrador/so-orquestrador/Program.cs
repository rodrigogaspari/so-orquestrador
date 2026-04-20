using so_orquestrador.Infrastructure.Services.Extensions;

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

        var app = builder.Build();

        // Activate Swagger documentation in developmant environment
        app.ActivateSwaggerInDebug(); 
       
        app.MapControllers();

        app.Run();
    }
}