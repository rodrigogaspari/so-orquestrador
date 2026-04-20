namespace so_orquestrador.WebApi.Contracts.Requests
{
    [Serializable]
    public class VendaRequest
    {
        public string? Cliente { get; set; }

        public decimal? Valor { get; set; }

        public string? IdentificacaoCliente { get; set; }
    }
}
