using Ftec.ProjetosWeb.Estatistica.Dominio.Entidades;
using System;
using System.Collections.Generic;

namespace Ftec.ProjetosWeb.Estatistica.Dominio.Interfaces
{
    public interface IEstatisticaRepositorio
    {
        List<EstatisticaVenda> ObterTopProdutosVendidos(DateTime data, int top);
        int ObterTotalNovosClientes(DateTime data);
        decimal ObterFaturamentoTotal(DateTime data);
        List<MediaAvaliacaoProduto> ObterMediaAvaliacaoProduto();
        List<MediaVendaPorProduto> ObterMediaVendaPorProduto();
        List<MediaVendasClientes> ObterMediaVendasClientes();
        TotalVendas ObterTotalVendas();
    }
}
