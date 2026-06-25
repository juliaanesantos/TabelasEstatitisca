using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Ftec.ProjetosWeb.Estatistica.Mvc.Models;
using Ftec.ProjetosWeb.Estatistica.Mvc.Services;

namespace Ftec.ProjetosWeb.Estatistica.Mvc.Controllers;

public class HomeController : Controller
{
    private readonly EstatisticaService _service;

    public HomeController(EstatisticaService service)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var model = new DashboardViewModel();

        try { model.Painel = await _service.GetPainelHojeAsync(); } catch { model.Painel = new List<EstatisticaResumoViewModel>(); }
        try { model.TotalVendas = await _service.GetTotalVendasAsync(); } catch { model.TotalVendas = new TotalVendasViewModel(); }

        return View(model);
    }

    public async Task<IActionResult> MediaAvaliacaoProduto()
    {
        try
        {
            var dados = await _service.GetMediaAvaliacaoProdutoAsync();
            return View(dados);
        }
        catch
        {
            return View(new List<MediaAvaliacaoProdutoViewModel>());
        }
    }

    public async Task<IActionResult> MediaVendaPorProduto()
    {
        try
        {
            var dados = await _service.GetMediaVendaPorProdutoAsync();
            return View(dados);
        }
        catch
        {
            return View(new List<MediaVendaPorProdutoViewModel>());
        }
    }

    public async Task<IActionResult> MediaVendasClientes()
    {
        try
        {
            var dados = await _service.GetMediaVendasClientesAsync();
            return View(dados);
        }
        catch
        {
            return View(new List<MediaVendasClientesViewModel>());
        }
    }

    public async Task<IActionResult> TotalVendas()
    {
        try
        {
            var dados = await _service.GetTotalVendasAsync();
            return View(dados);
        }
        catch
        {
            return View(new TotalVendasViewModel());
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
