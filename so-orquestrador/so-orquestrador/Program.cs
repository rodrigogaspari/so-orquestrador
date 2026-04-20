//using Swashbuckle.AspNetCore.Filters;
using Microsoft.OpenApi;
using so_orquestrador.Infrastructure.Services.Controllers;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        // HttpClients para cada serviço
        builder.Services.AddHttpClient("nota-fiscal-api", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7801");
        });

        builder.Services.AddHttpClient("conta-corrente-api", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7901");
        });



        // Adiciona serviços do Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSwaggerGen(c =>
        {
            c.ExampleFilters();

            c.OperationFilter<AddApiKeyHeaderParameter>();
        });


        builder.Services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "API Venda",
                Description = "Apresentação Smart Online - Gaspari",
            });
        });



        builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();


        var app = builder.Build();


        // Ativa o Swagger apenas em ambiente de desenvolvimento
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }


        app.MapControllers();

        app.Run();
    }
}