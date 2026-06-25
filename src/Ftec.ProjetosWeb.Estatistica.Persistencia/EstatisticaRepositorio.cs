using Ftec.ProjetosWeb.Estatistica.Dominio.Entidades;
using Ftec.ProjetosWeb.Estatistica.Dominio.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;

namespace Ftec.ProjetosWeb.Estatistica.Persistencia
{
    public class EstatisticaRepositorio : IEstatisticaRepositorio
    {
        private string stringConexao;

        public EstatisticaRepositorio(string strConexao)
        {
            stringConexao = strConexao;
        }

        public List<MediaAvaliacaoProduto> ObterMediaAvaliacaoProduto()
        {
            // Retorna lista vazia por enquanto. Implementar query se necessário.
            return new List<MediaAvaliacaoProduto>();
        }

        public List<MediaVendaPorProduto> ObterMediaVendaPorProduto()
        {
            return new List<MediaVendaPorProduto>();
        }

        public List<MediaVendasClientes> ObterMediaVendasClientes()
        {
            return new List<MediaVendasClientes>();
        }

        public TotalVendas ObterTotalVendas()
        {
            return new TotalVendas { TotalPedidos = 0, ValorTotalVendas = 0 };
        }

        public List<EstatisticaVenda> ObterTopProdutosVendidos(DateTime data, int top)
        {
            var lista = new List<EstatisticaVenda>();
            using (var conexao = new NpgsqlConnection(stringConexao))
            {
                conexao.Open();
                var comando = new NpgsqlCommand();
                comando.Connection = conexao;

                comando.CommandText = @"
                    SELECT p.nome, SUM(i.quantidade) as total_qtd, SUM(i.preco_unitario * i.quantidade) as total_valor
                    FROM public.pedido_itens i
                    INNER JOIN public.produtos p ON p.id = i.id_produto
                    INNER JOIN public.pedidos ped ON ped.id = i.id_pedido
                    WHERE ped.data_pedido::date = @data
                    GROUP BY p.nome
                    ORDER BY total_qtd DESC
                    LIMIT @top;";

                comando.Parameters.AddWithValue("data", data.Date);
                comando.Parameters.AddWithValue("top", top);

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new EstatisticaVenda
                        {
                            NomeProduto = reader["nome"].ToString(),
                            QuantidadeVendida = Convert.ToInt32(reader["total_qtd"]),
                            ValorTotal = Convert.ToDecimal(reader["total_valor"])
                        });
                    }
                }
            }
            return lista;
        }

        public int ObterTotalNovosClientes(DateTime data)
        {
            using (var conexao = new NpgsqlConnection(stringConexao))
            {
                conexao.Open();
                var comando = new NpgsqlCommand("SELECT COUNT(*) FROM public.clientes WHERE data_criacao::date = @data", conexao);
                comando.Parameters.AddWithValue("data", data.Date);
                return Convert.ToInt32(comando.ExecuteScalar());
            }
        }

        public decimal ObterFaturamentoTotal(DateTime data)
        {
            using (var conexao = new NpgsqlConnection(stringConexao))
            {
                conexao.Open();
                var comando = new NpgsqlCommand("SELECT SUM(valor_total) FROM public.pedidos WHERE data_pedido::date = @data", conexao);
                comando.Parameters.AddWithValue("data", data.Date);
                var result = comando.ExecuteScalar();
                return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
            }
        }
    }
}
