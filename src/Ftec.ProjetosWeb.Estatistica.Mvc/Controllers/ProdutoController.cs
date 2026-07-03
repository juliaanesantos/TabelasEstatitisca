using Microsoft.AspNetCore.Mvc;
using Ftec.ProjetosWeb.Estatistica.Mvc.Models;
using Ftec.ProjetosWeb.Estatistica.Mvc.Services;

namespace Ftec.ProjetosWeb.Estatistica.Mvc.Controllers;

public class ProdutoController : Controller
{
    private readonly ProdutoService _produto;
    private readonly EstatisticaService _estatistica;

    public ProdutoController(ProdutoService produto, EstatisticaService estatistica)
    {
        _produto = produto;
        _estatistica = estatistica;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var produtos = await _produto.ListarAsync();
            return View(produtos.Where(p => p.Disponivel && !string.IsNullOrEmpty(p.Nome)).ToList());
        }
        catch
        {
            return View(new List<ProdutoItemViewModel>());
        }
    }

    public async Task<IActionResult> Details(Guid id)
    {
        try
        {
            var produto = await _produto.ObterPorIdAsync(id);
            if (produto == null) return NotFound();

            var model = new ProdutoDetalheViewModel
            {
                Id = produto.Id,
                Codigo = produto.Codigo,
                Nome = produto.Nome,
                Preco = produto.Preco,
                Descricao = produto.Descricao,
                Disponivel = produto.Disponivel,
                Destaque = produto.Destaque,
                QuantidadeEstoque = produto.QuantidadeEstoque,
                IdCategoria = produto.IdCategoria,
                IdImagemPrincipal = produto.IdImagemPrincipal
            };

            try
            {
                var avaliacoes = await _estatistica.GetMediaAvaliacaoProdutoAsync();
                var aval = avaliacoes.FirstOrDefault(a => a.ProdutoId == id);
                if (aval != null)
                {
                    model.MediaAvaliacao = aval.MediaAvaliacao;
                    model.TotalAvaliacoes = aval.TotalAvaliacoes;
                }
            }
            catch { }

            return View(model);
        }
        catch
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> Search(string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return RedirectToAction("Index");

        try
        {
            var produtos = await _produto.BuscarAsync(q);
            ViewBag.SearchTerm = q;
            return View("Index", produtos.Where(p => p.Disponivel).ToList());
        }
        catch
        {
            return View("Index", new List<ProdutoItemViewModel>());
        }
    }
}
