using System.Text.Json;
using Ftec.ProjetosWeb.Estatistica.Mvc.Models;

namespace Ftec.ProjetosWeb.Estatistica.Mvc.Services;

public class PedidoService
{
    private readonly HttpClient _httpClient;

    public PedidoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<PedidoResumoViewModel>> ListarAsync()
    {
        var response = await _httpClient.GetAsync("/api/pedido");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<PedidoResumoViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
    }
}
