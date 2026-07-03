using System.Text.Json;

namespace Ftec.ProjetosWeb.Estatistica.Persistencia.ApiClientes
{
    public class ProdutoApiClient
    {
        private readonly HttpClient _httpClient;

        public ProdutoApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProdutoResponse>> ListarProdutosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/produto/listar");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var wrapper = JsonSerializer.Deserialize<ProdutoResponseWrapper>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return wrapper?.Data ?? new List<ProdutoResponse>();
            }
            catch
            {
                return new List<ProdutoResponse>();
            }
        }
    }
}
