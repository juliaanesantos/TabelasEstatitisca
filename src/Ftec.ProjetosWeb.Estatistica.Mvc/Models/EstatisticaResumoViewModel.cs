namespace Ftec.ProjetosWeb.Estatistica.Mvc.Models;

public class EstatisticaResumoViewModel
{
    public string Titulo { get; set; }
    public string Valor { get; set; }
    public string Detalhe { get; set; }
}

public class MediaAvaliacaoProdutoViewModel
{
    public Guid ProdutoId { get; set; }
    public string NomeProduto { get; set; }
    public decimal MediaAvaliacao { get; set; }
    public int TotalAvaliacoes { get; set; }
}

public class MediaVendaPorProdutoViewModel
{
    public Guid ProdutoId { get; set; }
    public string NomeProduto { get; set; }
    public int QuantidadeVendida { get; set; }
    public decimal ValorTotal { get; set; }
    public decimal MediaVenda { get; set; }
}

public class MediaVendasClientesViewModel
{
    public string NomeCliente { get; set; }
    public int QuantidadePedidos { get; set; }
    public decimal TotalVendas { get; set; }
    public decimal MediaVendas { get; set; }
}

public class TotalVendasViewModel
{
    public int TotalPedidos { get; set; }
    public decimal TotalVendas { get; set; }
}

public class DashboardViewModel
{
    public List<EstatisticaResumoViewModel> Painel { get; set; }
    public TotalVendasViewModel TotalVendas { get; set; }
}
