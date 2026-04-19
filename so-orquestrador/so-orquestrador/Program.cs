//using Swashbuckle.AspNetCore.Filters;
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
            client.BaseAddress = new Uri("https://localhost:7001");
        });

        builder.Services.AddHttpClient("conta-corrente-api", client =>
        {
            client.BaseAddress = new Uri("https://localhost:7002");
        });



        // Adiciona serviços do Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSwaggerGen(c =>
        {
            //c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            //{
            //    Title = "Minha API",
            //    Version = "v1"
            //});

            // Ativa suporte a exemplos
            c.ExampleFilters();
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