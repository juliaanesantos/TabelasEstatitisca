using Ftec.ProjetosWeb.Estatistica.Aplicacao.DTO;
using Ftec.ProjetosWeb.Estatistica.Dominio.Entidades;
using System.Collections.Generic;

namespace Ftec.ProjetosWeb.Estatistica.Aplicacao.Adapter
{
    public static class EstatisticaAdapter
    {
        public static EstatisticaResumoDTO ParaDTO(EstatisticaVenda entidade)
        {
            return new EstatisticaResumoDTO
            {
                Titulo = "Item Mais Vendido",
                Valor = entidade.NomeProduto,
                Detalhe = $"{entidade.QuantidadeVendida} unidades - Total: {entidade.ValorTotal:C}"
            };
        }

        public static EstatisticaResumoDTO ParaDTO(string titulo, object valor, string detalhe)
        {
            return new EstatisticaResumoDTO
            {
                Titulo = titulo,
                Valor = valor.ToString(),
                Detalhe = detalhe
            };
        }

        public static MediaAvaliacaoProdutoDTO ParaDTO(MediaAvaliacaoProduto entidade)
        {
            return new MediaAvaliacaoProdutoDTO
            {
                ProdutoId = entidade.ProdutoId,
                NomeProduto = entidade.NomeProduto,
                MediaAvaliacao = entidade.MediaAvaliacao,
                TotalAvaliacoes = entidade.TotalAvaliacoes
            };
        }

        public static List<MediaAvaliacaoProdutoDTO> ParaDTO(List<MediaAvaliacaoProduto> entidades)
        {
            var dto = new List<MediaAvaliacaoProdutoDTO>();
            foreach (var e in entidades)
                dto.Add(ParaDTO(e));
            return dto;
        }

        public static MediaVendaPorProdutoDTO ParaDTO(MediaVendaPorProduto entidade)
        {
            return new MediaVendaPorProdutoDTO
            {
                ProdutoId = entidade.ProdutoId,
                NomeProduto = entidade.NomeProduto,
                QuantidadeVendida = entidade.QuantidadeVendida,
                ValorTotal = entidade.ValorTotal,
                MediaVenda = entidade.MediaVenda
            };
        }

        public static List<MediaVendaPorProdutoDTO> ParaDTO(List<MediaVendaPorProduto> entidades)
        {
            var dto = new List<MediaVendaPorProdutoDTO>();
            foreach (var e in entidades)
                dto.Add(ParaDTO(e));
            return dto;
        }

        public static MediaVendasClientesDTO ParaDTO(MediaVendasClientes entidade)
        {
            return new MediaVendasClientesDTO
            {
                NomeCliente = entidade.NomeCliente,
                QuantidadePedidos = entidade.QuantidadePedidos,
                TotalVendas = entidade.TotalVendas,
                MediaVendas = entidade.MediaVendas
            };
        }

        public static List<MediaVendasClientesDTO> ParaDTO(List<MediaVendasClientes> entidades)
        {
            var dto = new List<MediaVendasClientesDTO>();
            foreach (var e in entidades)
                dto.Add(ParaDTO(e));
            return dto;
        }

        public static TotalVendasDTO ParaDTO(TotalVendas entidade)
        {
            return new TotalVendasDTO
            {
                TotalPedidos = entidade.TotalPedidos,
                TotalVendas = entidade.ValorTotalVendas
            };
        }
    }
}
