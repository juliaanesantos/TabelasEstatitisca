using System.Text.Json.Serialization;

namespace Ftec.ProjetosWeb.Estatistica.Persistencia.ApiClientes
{
    public class PedidoResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("usuarioId")]
        public string UsuarioId { get; set; }

        [JsonPropertyName("produtosModel")]
        public List<ProdutoPedidoResponse> ProdutosModel { get; set; }

        [JsonPropertyName("dataPedido")]
        public DateTime DataPedido { get; set; }

        [JsonPropertyName("statusPedido")]
        public int StatusPedido { get; set; }

        [JsonPropertyName("valorTotal")]
        public decimal ValorTotal { get; set; }
    }

    public class ProdutoPedidoResponse
    {
        [JsonPropertyName("produtoId")]
        public string ProdutoId { get; set; }

        [JsonPropertyName("quantidade")]
        public int Quantidade { get; set; }

        [JsonPropertyName("preco")]
        public decimal Preco { get; set; }
    }

    public class ProdutoResponseWrapper
    {
        [JsonPropertyName("sucesso")]
        public bool Sucesso { get; set; }

        [JsonPropertyName("data")]
        public List<ProdutoResponse> Data { get; set; }
    }

    public class ProdutoResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("preco")]
        public decimal Preco { get; set; }
    }

    public class LoginRequest
    {
        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("senha")]
        public string Senha { get; set; }
    }

    public class LoginResponse
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("accessTokenExpiresIn")]
        public DateTime AccessTokenExpiresIn { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("refreshTokenExpiresIn")]
        public DateTime RefreshTokenExpiresIn { get; set; }

        [JsonPropertyName("usuarioId")]
        public string UsuarioId { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class UsuarioResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("nomeFantasia")]
        public string NomeFantasia { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public class AvaliacaoResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("produtoId")]
        public string ProdutoId { get; set; }

        [JsonPropertyName("avaliacao")]
        public int Avaliacao { get; set; }

        [JsonPropertyName("dataAvaliacao")]
        public DateTime DataAvaliacao { get; set; }
    }
}
