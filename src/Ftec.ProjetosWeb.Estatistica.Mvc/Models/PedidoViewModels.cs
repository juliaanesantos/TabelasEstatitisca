namespace Ftec.ProjetosWeb.Estatistica.Mvc.Models;

public class PedidoResumoViewModel
{
    public Guid Id { get; set; }
    public int CodigoPedido { get; set; }
    public Guid UsuarioId { get; set; }
    public string NomeCliente { get; set; }
    public DateTime DataPedido { get; set; }
    public int StatusPedido { get; set; }
    public decimal ValorTotal { get; set; }
    public List<PedidoProdutoViewModel> ProdutosModel { get; set; }
}

public class PedidoProdutoViewModel
{
    public Guid Id { get; set; }
    public Guid ProdutoId { get; set; }
    public string NomeProduto { get; set; }
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
}
