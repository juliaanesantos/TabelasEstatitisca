using System.Text.Json;
using Ftec.ProjetosWeb.Estatistica.Mvc.Models;

namespace Ftec.ProjetosWeb.Estatistica.Mvc.Services;

public class EstatisticaService
{
    private readonly HttpClient _httpClient;

    public EstatisticaService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<EstatisticaResumoViewModel>> GetPainelHojeAsync()
    {
        var response = await _httpClient.GetAsync("/api/Estatistica/painel-hoje");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<EstatisticaResumoViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<List<MediaAvaliacaoProdutoViewModel>> GetMediaAvaliacaoProdutoAsync()
    {
        var response = await _httpClient.GetAsync("/api/Estatistica/media-avaliacao-produto");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<MediaAvaliacaoProdutoViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<List<MediaVendaPorProdutoViewModel>> GetMediaVendaPorProdutoAsync()
    {
        var response = await _httpClient.GetAsync("/api/Estatistica/media-venda-produto");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<MediaVendaPorProdutoViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<List<MediaVendasClientesViewModel>> GetMediaVendasClientesAsync()
    {
        var response = await _httpClient.GetAsync("/api/Estatistica/media-vendas-cliente");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<List<MediaVendasClientesViewModel>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    public async Task<TotalVendasViewModel> GetTotalVendasAsync()
    {
        var response = await _httpClient.GetAsync("/api/Estatistica/total-vendas");
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TotalVendasViewModel>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }
}
