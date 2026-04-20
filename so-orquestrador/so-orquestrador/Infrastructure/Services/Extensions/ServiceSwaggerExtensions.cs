using Microsoft.OpenApi;
using so_orquestrador.WebApi.SwaggerGen;
using Swashbuckle.AspNetCore.Filters;

namespace so_orquestrador.Infrastructure.Services.Extensions
{
    public static class ServiceSwaggerExtensions
    {
        public static void ConfigureSwaggerAppServices(this IServiceCollection services)
        {
            // Adiciona serviços do Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddSwaggerGen(c =>
            {
                c.ExampleFilters();

                c.OperationFilter<AddApiKeyHeaderParameter>();
            });

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "API Venda",
                    Description = "Apresentação Smart Online - Gaspari",
                });
            });

            services.AddSwaggerExamplesFromAssemblyOf<Program>();
        }
    }
}
