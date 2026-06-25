using Ftec.ProjetosWeb.Estatistica.Dominio.Entidades;
using Ftec.ProjetosWeb.Estatistica.Dominio.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Text;

namespace Ftec.ProjetosWeb.Estatistica.Persistencia
{
    public class MediaAvaliacaoProdutoRepositorio : IMediaAvaliacaoProdutoRepositorio
    {
        private string stringConexao;

        public MediaAvaliacaoProdutoRepositorio(string strConexao)
        {
            stringConexao = strConexao;
        }

        public List<MediaAvaliacaoProduto> ObterAvaliacoesProdutos(string nomeProduto, DateTime data)
        {
            var lista = new List<MediaAvaliacaoProduto>();
            using (var conexao = new NpgsqlConnection(stringConexao))
            {
                conexao.Open();
                var comando = new NpgsqlCommand();
                comando.Connection = conexao;

                comando.CommandText = @"SELECT COUNT(*) as quantidade, SUM(a.avaliacao) as soma, COUNT(*) as total, AVG(a.avaliacao) as media, a.data_avaliacao as data 
                                        FROM public.avaliacoes a
                                        INNER JOIN public.produtos p on p.id = a.id_produto
                                        WHERE p.nome = @nomeProduto AND a.data_avaliacao::date = @data
                                        GROUP BY a.data_avaliacao;";

                comando.Parameters.AddWithValue("nomeProduto", nomeProduto);
                comando.Parameters.AddWithValue("data", data.Date);

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var item = new MediaAvaliacaoProduto();
                        item.QuantidadeAvaliacao = Convert.ToInt32(reader["quantidade"]);
                        item.SomaAvaliacao = reader["soma"] == DBNull.Value ? 0 : Convert.ToInt32(reader["soma"]);
                        item.TotalAvaliacao = reader["total"] == DBNull.Value ? 0 : Convert.ToInt32(reader["total"]);
                        item.MediaAvaliacao = reader["media"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["media"]);
                        item.Data = Convert.ToDateTime(reader["data"]);
                        lista.Add(item);
                    }
                }
            }
            return lista;
        }

        public int ObterTotalAvalicoes(DateTime data)
        {
            using (var conexao = new NpgsqlConnection(stringConexao))
            {
                conexao.Open();
                var comando = new NpgsqlCommand("SELECT COUNT(*) FROM public.avaliacoes WHERE data_avaliacao::date = @data", conexao);
                comando.Parameters.AddWithValue("data", data.Date);
                return Convert.ToInt32(comando.ExecuteScalar());
            }
        }

        public decimal ObterMediaAvaliacoes(DateTime data)
        {
            using (var conexao = new NpgsqlConnection(stringConexao))
            {
                conexao.Open();
                var comando = new NpgsqlCommand("SELECT AVG(avaliacao) FROM public.avaliacoes WHERE data_avaliacao::date = @data", conexao);
                comando.Parameters.AddWithValue("data", data.Date);
                var result = comando.ExecuteScalar();
                return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
            }
        }
    }
}
