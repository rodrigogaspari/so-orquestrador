namespace so_orquestrador.Infrastructure.Services.Extensions
{
    public static class WebApplicationSwaggerExtensions
    {
        public static void ActivateSwaggerInDebug(this WebApplication app)
        {
            // Ativa o Swagger apenas em ambiente de desenvolvimento
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
        }
    }
}
