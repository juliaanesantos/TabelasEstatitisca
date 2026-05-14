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

        public decimal ObterFaturamentoTotal(DateTime data)
        {
            using (var conexao = new NpgsqlConnection(stringConexao))
            {
                conexao.Open();

                var comando = new NpgsqlCommand(
                    "SELECT SUM(valor_total) FROM public.pedido WHERE data_criacao::date = @data",
                    conexao);

                comando.Parameters.AddWithValue("data", data.Date);

                var result = comando.ExecuteScalar();

                return result == DBNull.Value ? 0 : Convert.ToDecimal(result);
            }
        }

        public int ObterTotalNovosClientes(DateTime data)
        {
            using (var conexao = new NpgsqlConnection(stringConexao))
            {
                conexao.Open();

                var comando = new NpgsqlCommand(
                    "SELECT COUNT(*) FROM public.cliente WHERE data_criacao::date = @data",
                    conexao);

                comando.Parameters.AddWithValue("data", data.Date);

                var result = comando.ExecuteScalar();

                return result == DBNull.Value ? 0 : Convert.ToInt32(result);
            }
        }

        public List<EstatisticaVenda> ObterTopProdutosVendidos(DateTime data, int top)
        {
            var lista = new List<EstatisticaVenda>();

            using (var conexao = new NpgsqlConnection(stringConexao))
            {
                conexao.Open();

                var comando = new NpgsqlCommand(@"
                    SELECT 
                        p.descricao AS nomeproduto,
                        SUM(pc.quantidade) AS quantidadependida,
                        SUM(pc.total) AS valortotal
                    FROM public.pedidocliente pc
                    INNER JOIN public.produto p ON p.id = pc.idproduto
                    INNER JOIN public.pedido ped ON ped.id = pc.idpedido
                    WHERE ped.data_criacao::date = @data
                    GROUP BY p.descricao
                    ORDER BY quantidadependida DESC
                    LIMIT @top", conexao);

                comando.Parameters.AddWithValue("data", data.Date);
                comando.Parameters.AddWithValue("top", top);

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new EstatisticaVenda
                        {
                            NomeProduto = reader["nomeproduto"].ToString(),
                            QuantidadeVendida = Convert.ToInt32(reader["quantidadependida"]),
                            ValorTotal = Convert.ToDecimal(reader["valortotal"]),
                            Data = data
                        });
                    }
                }
            }

            return lista;
        }

        public List<MediaAvaliacaoProduto> ObterMediaAvaliacaoProduto()
        {
            var lista = new List<MediaAvaliacaoProduto>();

            using (var conexao = new NpgsqlConnection(stringConexao))
            {
                conexao.Open();

                var comando = new NpgsqlCommand(@"
                    SELECT 
                        p.id AS produtoid,
                        p.descricao AS nomeproduto,
                        COALESCE(AVG(a.nota), 0) AS mediaavaliacao,
                        COUNT(a.id) AS totalavaliacoes
                    FROM public.produto p
                    LEFT JOIN public.avaliacao a ON a.produtoid = p.id
                    GROUP BY p.id, p.descricao
                    ORDER BY mediaavaliacao DESC", conexao);

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new MediaAvaliacaoProduto
                        {
                            ProdutoId = Guid.Parse(reader["produtoid"].ToString()),
                            NomeProduto = reader["nomeproduto"].ToString(),
                            MediaAvaliacao = Convert.ToDecimal(reader["mediaavaliacao"]),
                            TotalAvaliacoes = Convert.ToInt32(reader["totalavaliacoes"])
                        });
                    }
                }
            }

            return lista;
        }

        public List<MediaVendaPorProduto> ObterMediaVendaPorProduto()
        {
            var lista = new List<MediaVendaPorProduto>();

            using (var conexao = new NpgsqlConnection(stringConexao))
            {
                conexao.Open();

                var comando = new NpgsqlCommand(@"
                    SELECT 
                        p.id AS produtoid,
                        p.descricao AS nomeproduto,
                        COALESCE(SUM(pc.quantidade), 0) AS quantidadependida,
                        COALESCE(SUM(pc.total), 0) AS valortotal,
                        CASE
                            WHEN COUNT(pc.id) > 0 THEN COALESCE(SUM(pc.total) / COUNT(pc.id), 0)
                            ELSE 0
                        END AS mediavenda
                    FROM public.produto p
                    LEFT JOIN public.pedidocliente pc ON pc.idproduto = p.id
                    GROUP BY p.id, p.descricao
                    ORDER BY quantidadependida DESC", conexao);

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new MediaVendaPorProduto
                        {
                            ProdutoId = Guid.Parse(reader["produtoid"].ToString()),
                            NomeProduto = reader["nomeproduto"].ToString(),
                            QuantidadeVendida = Convert.ToInt32(reader["quantidadependida"]),
                            ValorTotal = Convert.ToDecimal(reader["valortotal"]),
                            MediaVenda = Convert.ToDecimal(reader["mediavenda"])
                        });
                    }
                }
            }

            return lista;
        }

        public List<MediaVendasClientes> ObterMediaVendasClientes()
        {
            var lista = new List<MediaVendasClientes>();

            using (var conexao = new NpgsqlConnection(stringConexao))
            {
                conexao.Open();

                var comando = new NpgsqlCommand(@"
                    SELECT 
                        c.id AS clienteid,
                        c.nome AS nomecliente,
                        COUNT(p.id) AS quantidadepedidos,
                        COALESCE(SUM(p.valor_total), 0) AS totalvendas,
                        CASE
                            WHEN COUNT(p.id) > 0 THEN COALESCE(SUM(p.valor_total) / COUNT(p.id), 0)
                            ELSE 0
                        END AS mediavendas
                    FROM public.cliente c
                    LEFT JOIN public.pedido p ON p.idcliente = c.id
                    GROUP BY c.id, c.nome
                    ORDER BY totalvendas DESC", conexao);

                using (var reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        lista.Add(new MediaVendasClientes
                        {
                            ClienteId = Guid.Parse(reader["clienteid"].ToString()),
                            NomeCliente = reader["nomecliente"].ToString(),
                            QuantidadePedidos = Convert.ToInt32(reader["quantidadepedidos"]),
                            TotalVendas = Convert.ToDecimal(reader["totalvendas"]),
                            MediaVendas = Convert.ToDecimal(reader["mediavendas"])
                        });
                    }
                }
            }

            return lista;
        }

        public TotalVendas ObterTotalVendas()
        {
            using (var conexao = new NpgsqlConnection(stringConexao))
            {
                conexao.Open();

                var comando = new NpgsqlCommand(@"
                    SELECT 
                        COUNT(id) AS totalpedidos,
                        COALESCE(SUM(valor_total), 0) AS totalvendas
                    FROM public.pedido", conexao);

                using (var reader = comando.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new TotalVendas
                        {
                            TotalPedidos = Convert.ToInt32(reader["totalpedidos"]),
                            ValorTotalVendas = Convert.ToDecimal(reader["totalvendas"])
                        };
                    }
                }
            }

            return new TotalVendas();
        }
    }
}
