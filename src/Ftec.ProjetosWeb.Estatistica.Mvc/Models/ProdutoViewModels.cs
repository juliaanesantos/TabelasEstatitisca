namespace Ftec.ProjetosWeb.Estatistica.Mvc.Models;

public class ProdutoItemViewModel
{
    public Guid Id { get; set; }
    public string Codigo { get; set; }
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public string Descricao { get; set; }
    public bool Disponivel { get; set; }
    public bool Destaque { get; set; }
    public int QuantidadeEstoque { get; set; }
    public Guid IdCategoria { get; set; }
    public Guid? IdImagemPrincipal { get; set; }
}

public class ProdutoDetalheViewModel
{
    public Guid Id { get; set; }
    public string Codigo { get; set; }
    public string Nome { get; set; }
    public decimal Preco { get; set; }
    public string Descricao { get; set; }
    public bool Disponivel { get; set; }
    public bool Destaque { get; set; }
    public int QuantidadeEstoque { get; set; }
    public Guid IdCategoria { get; set; }
    public Guid? IdImagemPrincipal { get; set; }
    public decimal MediaAvaliacao { get; set; }
    public int TotalAvaliacoes { get; set; }
}

public class ProdutoApiResponse
{
    public bool Sucesso { get; set; }
    public List<ProdutoItemViewModel> Data { get; set; }
}

public class ProdutoApiSingleResponse
{
    public bool Sucesso { get; set; }
    public ProdutoItemViewModel Data { get; set; }
}
