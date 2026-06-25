using Ftec.ProjetosWeb.Estatistica.Dominio.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ftec.ProjetosWeb.Estatistica.Dominio.Interfaces
{
    public interface IMediaAvaliacaoProdutoRepositorio
    {
        List<MediaAvaliacaoProduto> ObterAvaliacoesProdutos(string nomeProduto, DateTime data);
        int ObterTotalAvalicoes(DateTime data);
        decimal ObterMediaAvaliacoes(DateTime data);

    }
}
