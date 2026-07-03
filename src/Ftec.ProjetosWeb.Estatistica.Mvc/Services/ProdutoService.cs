using System.Text.Json;
using Ftec.ProjetosWeb.Estatistica.Mvc.Models;

namespace Ftec.ProjetosWeb.Estatistica.Mvc.Services;

public class ProdutoService
{
    private readonly HttpClient _httpClient;

    public ProdutoService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<ProdutoItemViewModel>> ListarAsync()
    {
        var response = await _httpClient.GetAsync("/api/produto/listar");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var wrapper = JsonSerializer.Deserialize<ProdutoApiResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return wrapper?.Data ?? new List<ProdutoItemViewModel>();
    }

    public async Task<ProdutoItemViewModel> ObterPorIdAsync(Guid id)
    {
        var response = await _httpClient.GetAsync($"/api/produto/obtemPorId/{id}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var wrapper = JsonSerializer.Deserialize<ProdutoApiSingleResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return wrapper?.Data;
    }

    public async Task<List<ProdutoItemViewModel>> BuscarAsync(string texto)
    {
        var response = await _httpClient.GetAsync($"/api/produto/buscar/{texto}");
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var wrapper = JsonSerializer.Deserialize<ProdutoApiResponse>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        return wrapper?.Data ?? new List<ProdutoItemViewModel>();
    }
}
