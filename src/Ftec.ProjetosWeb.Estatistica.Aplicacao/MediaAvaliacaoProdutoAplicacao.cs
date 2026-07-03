using Ftec.ProjetosWeb.Estatistica.Aplicacao.Adapter;
using Ftec.ProjetosWeb.Estatistica.Aplicacao.DTO;
using Ftec.ProjetosWeb.Estatistica.Dominio.Interfaces;

namespace Ftec.ProjetosWeb.Estatistica.Aplicacao
{
    public class MediaAvaliacaoProdutoAplicacao
    {
        private readonly IMediaAvaliacaoProdutoRepositorio repositorio;

        public MediaAvaliacaoProdutoAplicacao(IMediaAvaliacaoProdutoRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        public List<MediaAvaliacaoProdutoDTO> ListarAvaliacao(string nomeProduto, DateTime data)
        {
            var avaliacoes = repositorio.ObterAvaliacoesProdutos(nomeProduto, data);
            var dtos = new List<MediaAvaliacaoProdutoDTO>();

            foreach (var ava in avaliacoes)
                dtos.Add(MediaAvaliacaoProdutoAdapter.ParaDTO(ava));

            return dtos;
        }

        public int ObterTotalAvalicoes(DateTime data)
        {
            return repositorio.ObterTotalAvalicoes(data);
        }

        public decimal ObterMediaAvaliacoes(DateTime data)
        {
            return repositorio.ObterMediaAvaliacoes(data);
        }
    }
}
