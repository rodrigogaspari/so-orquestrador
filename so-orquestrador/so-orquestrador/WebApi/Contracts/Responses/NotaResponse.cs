namespace so_orquestrador.WebApi.Contracts.Responses
{
    [Serializable]
    public class NotaResponse
    {
        public string? IdentificacaoCliente { get; set; }

        public string? Cliente { get; set; }

        public string? Numero { get; set; }

        public decimal? Valor { get; set; }

        public string? Chave { get; set; }
    }
}
