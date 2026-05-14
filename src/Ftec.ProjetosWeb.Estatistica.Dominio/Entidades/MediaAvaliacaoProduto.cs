using System;

namespace Ftec.ProjetosWeb.Estatistica.Dominio.Entidades
{
    public class MediaAvaliacaoProduto
    {
        public Guid ProdutoId { get; set; }
        public string NomeProduto { get; set; }
        public decimal MediaAvaliacao { get; set; }
        public int TotalAvaliacoes { get; set; }
    }
}
