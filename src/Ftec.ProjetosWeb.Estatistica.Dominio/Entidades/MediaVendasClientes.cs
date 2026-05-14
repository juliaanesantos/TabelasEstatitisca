using System;

namespace Ftec.ProjetosWeb.Estatistica.Dominio.Entidades
{
    public class MediaVendasClientes
    {
        public Guid ClienteId { get; set; }
        public string NomeCliente { get; set; }
        public int QuantidadePedidos { get; set; }
        public decimal TotalVendas { get; set; }
        public decimal MediaVendas { get; set; }
    }
}
