using Microsoft.AspNetCore.Mvc;
using Ftec.ProjetosWeb.Estatistica.Mvc.Services;

namespace Ftec.ProjetosWeb.Estatistica.Mvc.Controllers;

public class PedidoController : Controller
{
    private readonly PedidoService _pedido;

    public PedidoController(PedidoService pedido)
    {
        _pedido = pedido;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var pedidos = await _pedido.ListarAsync();
            pedidos = pedidos.OrderByDescending(p => p.DataPedido).ToList();
            return View(pedidos);
        }
        catch
        {
            return View(new List<Ftec.ProjetosWeb.Estatistica.Mvc.Models.PedidoResumoViewModel>());
        }
    }
}
