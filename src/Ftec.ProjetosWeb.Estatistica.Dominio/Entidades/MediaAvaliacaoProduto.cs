using System;

namespace Ftec.ProjetosWeb.Estatistica.Dominio.Entidades
{
    public class MediaAvaliacaoProduto
    {
        public MediaAvaliacaoProduto()
        {
            // valores padrão
            QuantidadeAvaliacao = 0;
            SomaAvaliacao = 0;
            TotalAvaliacao = 0;
            MediaAvaliacao = 0;
            Data = DateTime.MinValue;
            ProdutoId = Guid.Empty;
            NomeProduto = string.Empty;
            TotalAvaliacoes = 0;
        }

        public int QuantidadeAvaliacao { get; set; }
        public int SomaAvaliacao { get; set; }
        public int TotalAvaliacao { get; set; }
        public decimal MediaAvaliacao { get; set; }
        public DateTime Data { get; set; }

        // Adicionados para compatibilidade com DTOs e adaptadores
        public Guid ProdutoId { get; set; }
        public string NomeProduto { get; set; }
        public int TotalAvaliacoes { get; set; }


    }
}
