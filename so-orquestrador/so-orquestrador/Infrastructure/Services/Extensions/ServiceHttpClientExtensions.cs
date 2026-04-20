namespace so_orquestrador.Infrastructure.Services.Extensions
{
    public static class ServiceHttpClientExtensions
    {
        public static void AddHttpClients(this IServiceCollection services)
        {
            // HttpClients para cada serviço
            services.AddHttpClient("nota-fiscal-api", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7801");
            });

            services.AddHttpClient("conta-corrente-api", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7901");
            });
        }
    }
}
