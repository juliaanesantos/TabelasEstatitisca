using System.Text.Json;

namespace Ftec.ProjetosWeb.Estatistica.Persistencia.ApiClientes
{
    public class PedidoApiClient
    {
        private readonly HttpClient _httpClient;

        public PedidoApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<PedidoResponse>> ListarPedidosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("/api/Pedido");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<PedidoResponse>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<PedidoResponse>();
            }
            catch
            {
                return new List<PedidoResponse>();
            }
        }
    }
}
