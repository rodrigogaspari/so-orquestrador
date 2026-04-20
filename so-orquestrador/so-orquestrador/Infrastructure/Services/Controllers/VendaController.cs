using Microsoft.AspNetCore.Mvc;
using so_orquestrador.Infrastructure.Queue.Util;
using so_orquestrador.WebApi.Contracts.Requests;
using so_orquestrador.WebApi.Contracts.Responses;
using so_orquestrador.WebApi.SwaggerGen;
using Swashbuckle.AspNetCore.Filters;
using System.Net;
using System.Text.Json;

namespace so_orquestrador.Infrastructure.Services.Controllers
{
    [ApiController()]
    [Route("api/v1")]
    public class VendaController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public VendaController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        /// <summary>
        /// Serviço: Venda
        /// </summary>
        /// <remarks>
        ///         
        ///</remarks>
        /// <param name="request">Corpo da requisição do recurso.</param>
        /// <response code="200">Retorna sucesso na consulta.</response>
        /// <response code="400">Se houver algum tipo de problema/validação na consulta.</response>
        /// <response code="422">Se houver algum tipo de erro com os dados da operação.</response>
        /// <response code="503">Se houver algum tipo de erro ao chamar serviços externos.</response>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        [SwaggerRequestExample(typeof(VendaRequest), typeof(CreateVendaRequestExample))]
        [Route("venda")]
        [HttpPost()]
        public async Task<ActionResult<VendaResponse>> Post(
            VendaRequest request, [FromHeader] string idempotencyKey
            )
        {

            var errosDados = new List<string>();
            var errosChamadaApi = new List<string>();

            var notaFiscalClient = _httpClientFactory.CreateClient("nota-fiscal-api");
            var contaCorrenteClient = _httpClientFactory.CreateClient("conta-corrente-api");

            var notaFiscal = new NotaResponse();
            var respostaNota = new HttpResponseMessage();

            var vendaResponse = new VendaResponse();

            try
            {
                notaFiscalClient.DefaultRequestHeaders.Add("IdempotencyKey", idempotencyKey);

                respostaNota = await notaFiscalClient.PostAsJsonAsync("/api/v1/notafiscal/nota", request);

                if (respostaNota != null && HttpStatusCode.OK.Equals(respostaNota.StatusCode))
                {
                    var respostaNotaJson = await respostaNota.Content.ReadAsStringAsync();

                    notaFiscal = JsonSerializer.Deserialize<NotaResponse>(respostaNotaJson,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                }
                else
                    errosDados.Add("Não foi possível emitir a NF-e do cliente. Verifique os dados da venda.");

            }
            catch
            {
                errosChamadaApi.Add("Erro ao chamar a api de emissão de NF-e.");
            }


            try
            {
                if (!errosChamadaApi.Any() && !errosDados.Any())
                {
                    contaCorrenteClient.DefaultRequestHeaders.Add("IdempotencyKey", idempotencyKey);

                    var responteConta = await contaCorrenteClient.PostAsJsonAsync($"/api/v1/contacorrente/{request.IdentificacaoCliente}/movimento", new { TipoMovimento = "D", Valor = request.Valor });

                    if (responteConta != null && HttpStatusCode.OK.Equals(responteConta.StatusCode))
                    {
                        vendaResponse.IdentificacaoCliente = request.IdentificacaoCliente;
                        vendaResponse.Cliente = notaFiscal.Cliente;
                        vendaResponse.Numero = notaFiscal.Numero;
                        vendaResponse.Valor = request.Valor;
                        vendaResponse.ChaveNFe = notaFiscal.Chave;
                    }
                    else
                    {
                        // Retorna informações sobre o problema com o lançamento
                        errosDados.Add("Não foi possível realizar o lançamento de débito do cliente. Verifique os dados do cliente.");
                        errosDados.Add(await responteConta.Content.ReadAsStringAsync());
                        errosDados.Add("Será realizado o cancelamento do documento emitido.");

                        // Gravar na fila de cancelamento para que NF-e seja cancelada
                        QueueUtil.PublicarMensagem(QueueUtil.FilaCancelar, notaFiscal);
                    }
                }
            }
            catch
            {
                errosChamadaApi.Add("Erro ao chamar a api de conta do cliente.");
            }



            if (errosChamadaApi.Any())
                return StatusCode(503, new
                {
                    sucesso = false,
                    mensagem = "Ocorreu algum erro durante a chamada de serviços externos.",
                    errosChamadaApi
                });


            if (errosDados.Any())
                return StatusCode(422, new
                {
                    sucesso = false,
                    mensagem = "Ocorreu algum erro relacionado aos dados da operação.",
                    errosDados
                });

            return Ok(vendaResponse);
        }
    }
}
