using Ftec.ProjetosWeb.Estatistica.Aplicacao.Adapter;
using Ftec.ProjetosWeb.Estatistica.Aplicacao.DTO;
using Ftec.ProjetosWeb.Estatistica.Dominio.Interfaces;

namespace Ftec.ProjetosWeb.Estatistica.Aplicacao
{
    public class EstatisticaAplicacao
    {
        private readonly IEstatisticaRepositorio _repositorio;

        public EstatisticaAplicacao(IEstatisticaRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public List<EstatisticaResumoDTO> GerarPainelDiario(DateTime data)
        {
            var painel = new List<EstatisticaResumoDTO>();

            int clientes = _repositorio.ObterTotalNovosClientes(data);
            painel.Add(EstatisticaAdapter.ParaDTO("Novos Clientes", clientes, "Contas criadas hoje"));

            decimal total = _repositorio.ObterFaturamentoTotal(data);
            painel.Add(EstatisticaAdapter.ParaDTO("Faturamento", total.ToString("C"), "Total de vendas em R$"));

            var tops = _repositorio.ObterTopProdutosVendidos(data, 1);
            if (tops.Count > 0)
            {
                painel.Add(EstatisticaAdapter.ParaDTO(tops[0]));
            }

            return painel;
        }

        public List<MediaAvaliacaoProdutoDTO> ObterMediaAvaliacaoProduto()
        {
            return EstatisticaAdapter.ParaDTO(_repositorio.ObterMediaAvaliacaoProduto());
        }

        public List<MediaVendaPorProdutoDTO> ObterMediaVendaPorProduto()
        {
            return EstatisticaAdapter.ParaDTO(_repositorio.ObterMediaVendaPorProduto());
        }

        public List<MediaVendasClientesDTO> ObterMediaVendasClientes()
        {
            return EstatisticaAdapter.ParaDTO(_repositorio.ObterMediaVendasClientes());
        }

        public TotalVendasDTO ObterTotalVendas()
        {
            return EstatisticaAdapter.ParaDTO(_repositorio.ObterTotalVendas());
        }
    }
}
