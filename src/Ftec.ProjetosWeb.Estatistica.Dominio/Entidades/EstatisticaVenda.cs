using System;

namespace Ftec.ProjetosWeb.Estatistica.Dominio.Entidades
{
    public class EstatisticaVenda
    {
        public string NomeProduto { get; set; }
        public int QuantidadeVendida { get; set; }
        public decimal ValorTotal { get; set; }
        public DateTime Data { get; set; }
    }
}
