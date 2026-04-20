using so_orquestrador.WebApi.Contracts.Requests;
using Swashbuckle.AspNetCore.Filters;

namespace so_orquestrador.WebApi.SwaggerGen
{
    [Serializable]
    public class CreateVendaRequestExample : IExamplesProvider<VendaRequest>
    {
        public VendaRequest GetExamples()
        {
            return new VendaRequest
            {
                Cliente = "José da Silva",
                IdentificacaoCliente = "000.001.002-03",
                Valor = 100m
            };
        }
    }
}
