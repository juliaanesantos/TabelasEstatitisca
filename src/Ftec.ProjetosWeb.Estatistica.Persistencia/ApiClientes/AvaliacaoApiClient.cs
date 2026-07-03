using System.Text.Json;

namespace Ftec.ProjetosWeb.Estatistica.Persistencia.ApiClientes
{
    public class AvaliacaoApiClient
    {
        private readonly HttpClient _httpClient;

        public AvaliacaoApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<AvaliacaoResponse>> ListarAvaliacoesPorProdutoAsync(Guid produtoId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"/api/avaliacao/produto/{produtoId}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<AvaliacaoResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<AvaliacaoResponse>();
            }
            catch
            {
                return new List<AvaliacaoResponse>();
            }
        }

        public async Task<List<AvaliacaoResponse>> ListarTodasAvaliacoesAsync(List<ProdutoResponse> produtos)
        {
            var todas = new List<AvaliacaoResponse>();
            foreach (var produto in produtos)
            {
                if (string.IsNullOrEmpty(produto?.Id)) continue;
                if (!Guid.TryParse(produto.Id, out var pid)) continue;
                var avaliacoes = await ListarAvaliacoesPorProdutoAsync(pid);
                todas.AddRange(avaliacoes);
            }
            return todas;
        }
    }
}
