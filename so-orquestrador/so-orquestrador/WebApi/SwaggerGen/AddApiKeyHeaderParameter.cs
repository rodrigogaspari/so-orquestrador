using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.Json.Nodes;

namespace so_orquestrador.WebApi.SwaggerGen
{
    public class AddApiKeyHeaderParameter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters.Where(p => p.Name.Equals("idempotencyKey")).Any())
                operation.Parameters.Remove(operation.Parameters.Where(p => p.Name.Equals("idempotencyKey")).FirstOrDefault());

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "idempotencyKey",
                In = ParameterLocation.Header,
                Required = true,
                Description = "Chave de idempotência para uso do endpoint.",
                Example = JsonValue.Create(Guid.NewGuid().ToString())
            });

        }
    }
}
