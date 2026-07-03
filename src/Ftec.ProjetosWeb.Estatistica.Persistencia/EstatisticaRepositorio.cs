using Ftec.ProjetosWeb.Estatistica.Dominio.Entidades;
using Ftec.ProjetosWeb.Estatistica.Dominio.Interfaces;
using Ftec.ProjetosWeb.Estatistica.Persistencia.ApiClientes;
using Npgsql;
using System.Text.Json;

namespace Ftec.ProjetosWeb.Estatistica.Persistencia
{
    public class EstatisticaRepositorio : IEstatisticaRepositorio
    {
        private readonly string _stringConexao;
        private readonly PedidoApiClient _pedidoClient;
        private readonly ProdutoApiClient _produtoClient;
        private readonly UsuarioApiClient _usuarioClient;
        private readonly AvaliacaoApiClient _avaliacaoClient;

        public EstatisticaRepositorio(
            string stringConexao,
            PedidoApiClient pedidoClient,
            ProdutoApiClient produtoClient,
            UsuarioApiClient usuarioClient,
            AvaliacaoApiClient avaliacaoClient)
        {
            _stringConexao = stringConexao;
            _pedidoClient = pedidoClient;
            _produtoClient = produtoClient;
            _usuarioClient = usuarioClient;
            _avaliacaoClient = avaliacaoClient;
        }

        public async Task<List<EstatisticaVenda>> ObterTopProdutosVendidos(DateTime data, int top)
        {
            await SincronizarPainelDiario(data);
            return await ConsultarTopProdutosLocais(data, top);
        }

        public async Task<int> ObterTotalNovosClientes(DateTime data)
        {
            await SincronizarPainelDiario(data);
            return await ConsultarTotalNovosClientesLocais(data);
        }

        public async Task<decimal> ObterFaturamentoTotal(DateTime data)
        {
            await SincronizarPainelDiario(data);
            return await ConsultarFaturamentoTotalLocal(data);
        }

        public async Task<List<MediaAvaliacaoProduto>> ObterMediaAvaliacaoProduto()
        {
            await SincronizarMediaAvaliacao();
            return await ConsultarMediaAvaliacaoLocal();
        }

        public async Task<List<MediaVendaPorProduto>> ObterMediaVendaPorProduto()
        {
            await SincronizarMediaVendaProduto();
            return await ConsultarMediaVendaProdutoLocal();
        }

        public async Task<List<MediaVendasClientes>> ObterMediaVendasClientes()
        {
            await SincronizarMediaVendasCliente();
            return await ConsultarMediaVendasClienteLocal();
        }

        public async Task<TotalVendas> ObterTotalVendas()
        {
            await SincronizarTotalVendas();
            return await ConsultarTotalVendasLocal();
        }

        private async Task SincronizarPainelDiario(DateTime data)
        {
            var pedidos = await _pedidoClient.ListarPedidosAsync();

            var pedidosHoje = pedidos.Where(p => p.DataPedido.Date == data.Date).ToList();

            var totalClientes = pedidosHoje.Select(p => p.UsuarioId).Distinct().Count();
            var faturamento = pedidosHoje.Sum(p => p.ValorTotal);

            using var conexao = new NpgsqlConnection(_stringConexao);
            await conexao.OpenAsync();

            await using (var cmd = new NpgsqlCommand("DELETE FROM public.painel_diario WHERE data_referencia = @data", conexao))
            {
                cmd.Parameters.AddWithValue("data", data.Date);
                await cmd.ExecuteNonQueryAsync();
            }

            await using (var cmd = new NpgsqlCommand(
                @"INSERT INTO public.painel_diario (indicador, valor, detalhe, data_referencia)
                  VALUES (@ind, @val, @det, @data)", conexao))
            {
                cmd.Parameters.AddWithValue("ind", "Novos Clientes");
                cmd.Parameters.AddWithValue("val", totalClientes.ToString());
                cmd.Parameters.AddWithValue("det", "Contas criadas hoje");
                cmd.Parameters.AddWithValue("data", data.Date);
                await cmd.ExecuteNonQueryAsync();
            }

            await using (var cmd = new NpgsqlCommand(
                @"INSERT INTO public.painel_diario (indicador, valor, detalhe, data_referencia)
                  VALUES (@ind, @val, @det, @data)", conexao))
            {
                cmd.Parameters.AddWithValue("ind", "Faturamento");
                cmd.Parameters.AddWithValue("val", faturamento.ToString("F2"));
                cmd.Parameters.AddWithValue("det", "Total de vendas em R$");
                cmd.Parameters.AddWithValue("data", data.Date);
                await cmd.ExecuteNonQueryAsync();
            }

            var produtos = await _produtoClient.ListarProdutosAsync();
            var produtosMap = produtos.ToDictionary(p => Guid.Parse(p.Id), p => p.Nome);

            var topProduto = pedidosHoje
                .SelectMany(p => p.ProdutosModel ?? new List<ProdutoPedidoResponse>())
                .GroupBy(pp => pp.ProdutoId)
                .Select(g => new
                {
                    ProdutoId = Guid.Parse(g.Key),
                    TotalQtd = g.Sum(pp => pp.Quantidade),
                    TotalValor = g.Sum(pp => pp.Quantidade * pp.Preco)
                })
                .OrderByDescending(x => x.TotalQtd)
                .FirstOrDefault();

            if (topProduto != null)
            {
                var nome = produtosMap.ContainsKey(topProduto.ProdutoId) ? produtosMap[topProduto.ProdutoId] : "Desconhecido";
                await using (var cmd = new NpgsqlCommand(
                    @"INSERT INTO public.painel_diario (indicador, valor, detalhe, data_referencia)
                      VALUES (@ind, @val, @det, @data)", conexao))
                {
                    cmd.Parameters.AddWithValue("ind", "Item Mais Vendido");
                    cmd.Parameters.AddWithValue("val", nome);
                    cmd.Parameters.AddWithValue("det", $"{topProduto.TotalQtd} unidades - Total: {topProduto.TotalValor:C}");
                    cmd.Parameters.AddWithValue("data", data.Date);
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        private async Task<List<EstatisticaVenda>> ConsultarTopProdutosLocais(DateTime data, int top)
        {
            var lista = new List<EstatisticaVenda>();
            using var conexao = new NpgsqlConnection(_stringConexao);
            await conexao.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                @"SELECT valor as nomeproduto, detalhe
                  FROM public.painel_diario
                  WHERE indicador = 'Item Mais Vendido' AND data_referencia = @data
                  LIMIT @top", conexao);
            cmd.Parameters.AddWithValue("data", data.Date);
            cmd.Parameters.AddWithValue("top", top);

            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new EstatisticaVenda
                {
                    NomeProduto = reader["nomeproduto"].ToString(),
                    QuantidadeVendida = 0,
                    ValorTotal = 0,
                    Data = data
                });
            }
            return lista;
        }

        private async Task<int> ConsultarTotalNovosClientesLocais(DateTime data)
        {
            using var conexao = new NpgsqlConnection(_stringConexao);
            await conexao.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "SELECT valor FROM public.painel_diario WHERE indicador = 'Novos Clientes' AND data_referencia = @data", conexao);
            cmd.Parameters.AddWithValue("data", data.Date);
            var result = await cmd.ExecuteScalarAsync();
            return result != null && int.TryParse(result.ToString(), out var val) ? val : 0;
        }

        private async Task<decimal> ConsultarFaturamentoTotalLocal(DateTime data)
        {
            using var conexao = new NpgsqlConnection(_stringConexao);
            await conexao.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "SELECT valor FROM public.painel_diario WHERE indicador = 'Faturamento' AND data_referencia = @data", conexao);
            cmd.Parameters.AddWithValue("data", data.Date);
            var result = await cmd.ExecuteScalarAsync();
            return result != null && decimal.TryParse(result.ToString(), out var val) ? val : 0;
        }

        private async Task SincronizarMediaAvaliacao()
        {
            var produtos = await _produtoClient.ListarProdutosAsync();
            var produtosValidos = produtos.Where(p => !string.IsNullOrEmpty(p?.Id)).ToList();
            var produtosMap = new Dictionary<Guid, string>();
            foreach (var p in produtosValidos)
            {
                if (Guid.TryParse(p.Id, out var pid))
                    produtosMap[pid] = p.Nome;
            }

            var avaliacoes = await _avaliacaoClient.ListarTodasAvaliacoesAsync(produtos);

            using var conexao = new NpgsqlConnection(_stringConexao);
            await conexao.OpenAsync();

            await using (var cmd = new NpgsqlCommand("DELETE FROM public.media_avaliacao_produto", conexao))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            var grupos = avaliacoes
                .Where(a => !string.IsNullOrEmpty(a.ProdutoId) && Guid.TryParse(a.ProdutoId, out _))
                .GroupBy(a => a.ProdutoId)
                .Select(g =>
                {
                    var pid = Guid.Parse(g.Key);
                    return new
                    {
                        ProdutoId = pid,
                        ProdutoNome = produtosMap.ContainsKey(pid) ? produtosMap[pid] : "Desconhecido",
                        Media = Math.Round(g.Average(a => a.Avaliacao), 2),
                        Total = g.Count()
                    };
                });

            foreach (var g in grupos)
            {
                await using var cmd = new NpgsqlCommand(
                    @"INSERT INTO public.media_avaliacao_produto (produtoid, nomeproduto, mediaavaliacao, totalavaliacoes)
                      VALUES (@pid, @pnome, @media, @total)", conexao);
                cmd.Parameters.AddWithValue("pid", g.ProdutoId);
                cmd.Parameters.AddWithValue("pnome", g.ProdutoNome);
                cmd.Parameters.AddWithValue("media", g.Media);
                cmd.Parameters.AddWithValue("total", g.Total);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task<List<MediaAvaliacaoProduto>> ConsultarMediaAvaliacaoLocal()
        {
            var lista = new List<MediaAvaliacaoProduto>();
            using var conexao = new NpgsqlConnection(_stringConexao);
            await conexao.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "SELECT produtoid, nomeproduto, mediaavaliacao, totalavaliacoes FROM public.media_avaliacao_produto ORDER BY mediaavaliacao DESC", conexao);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new MediaAvaliacaoProduto
                {
                    ProdutoId = reader["produtoid"] == DBNull.Value ? Guid.Empty : Guid.Parse(reader["produtoid"].ToString()),
                    NomeProduto = reader["nomeproduto"].ToString(),
                    MediaAvaliacao = Convert.ToDecimal(reader["mediaavaliacao"]),
                    TotalAvaliacoes = Convert.ToInt32(reader["totalavaliacoes"])
                });
            }
            return lista;
        }

        private async Task SincronizarMediaVendaProduto()
        {
            var pedidos = await _pedidoClient.ListarPedidosAsync();
            var produtos = await _produtoClient.ListarProdutosAsync();
            var produtosMap = produtos.ToDictionary(p => Guid.Parse(p.Id), p => p.Nome);

            using var conexao = new NpgsqlConnection(_stringConexao);
            await conexao.OpenAsync();

            await using (var cmd = new NpgsqlCommand("DELETE FROM public.media_venda_produto", conexao))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            var grupos = pedidos
                .SelectMany(p => p.ProdutosModel ?? new List<ProdutoPedidoResponse>())
                .GroupBy(pp => pp.ProdutoId)
                .Select(g =>
                {
                    var pid = Guid.Parse(g.Key);
                    return new
                    {
                        ProdutoId = pid,
                        NomeProduto = produtosMap.ContainsKey(pid) ? produtosMap[pid] : "Desconhecido",
                        Quantidade = g.Sum(pp => pp.Quantidade),
                        ValorTotal = g.Sum(pp => pp.Quantidade * pp.Preco),
                        MediaVenda = g.Sum(pp => pp.Quantidade * pp.Preco) / (g.Sum(pp => pp.Quantidade) > 0 ? g.Sum(pp => pp.Quantidade) : 1)
                    };
                });

            foreach (var g in grupos)
            {
                await using var cmd = new NpgsqlCommand(
                    @"INSERT INTO public.media_venda_produto (produtoid, nomeproduto, quantidadevendida, valortotal, mediavenda)
                      VALUES (@pid, @pnome, @qtd, @vtotal, @media)", conexao);
                cmd.Parameters.AddWithValue("pid", g.ProdutoId);
                cmd.Parameters.AddWithValue("pnome", g.NomeProduto);
                cmd.Parameters.AddWithValue("qtd", g.Quantidade);
                cmd.Parameters.AddWithValue("vtotal", g.ValorTotal);
                cmd.Parameters.AddWithValue("media", Math.Round(g.MediaVenda, 2));
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task<List<MediaVendaPorProduto>> ConsultarMediaVendaProdutoLocal()
        {
            var lista = new List<MediaVendaPorProduto>();
            using var conexao = new NpgsqlConnection(_stringConexao);
            await conexao.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "SELECT produtoid, nomeproduto, quantidadevendida, valortotal, mediavenda FROM public.media_venda_produto ORDER BY quantidadevendida DESC", conexao);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new MediaVendaPorProduto
                {
                    ProdutoId = reader["produtoid"] == DBNull.Value ? Guid.Empty : Guid.Parse(reader["produtoid"].ToString()),
                    NomeProduto = reader["nomeproduto"].ToString(),
                    QuantidadeVendida = Convert.ToInt32(reader["quantidadevendida"]),
                    ValorTotal = Convert.ToDecimal(reader["valortotal"]),
                    MediaVenda = Convert.ToDecimal(reader["mediavenda"])
                });
            }
            return lista;
        }

        private async Task SincronizarMediaVendasCliente()
        {
            var pedidos = await _pedidoClient.ListarPedidosAsync();
            var usuariosIds = pedidos.Select(p => p.UsuarioId).Distinct().ToList();

            using var conexao = new NpgsqlConnection(_stringConexao);
            await conexao.OpenAsync();

            await using (var cmd = new NpgsqlCommand("DELETE FROM public.media_vendas_cliente", conexao))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            var grupos = pedidos
                .GroupBy(p => p.UsuarioId)
                .Select(async g =>
                {
                    var clienteId = Guid.Parse(g.Key);
                    var nome = await _usuarioClient.ObterNomeUsuarioAsync(clienteId);
                    return new
                    {
                        ClienteId = clienteId,
                        NomeCliente = nome,
                        QuantidadePedidos = g.Count(),
                        TotalVendas = g.Sum(p => p.ValorTotal),
                        MediaVendas = g.Count() > 0 ? g.Sum(p => p.ValorTotal) / g.Count() : 0
                    };
                });

            var resultados = await Task.WhenAll(grupos);

            foreach (var r in resultados)
            {
                await using var cmd = new NpgsqlCommand(
                    @"INSERT INTO public.media_vendas_cliente (clienteid, nomecliente, quantidadepedidos, totalvendas, mediavendas)
                      VALUES (@cid, @cnome, @qtd, @vtotal, @media)", conexao);
                cmd.Parameters.AddWithValue("cid", r.ClienteId);
                cmd.Parameters.AddWithValue("cnome", r.NomeCliente);
                cmd.Parameters.AddWithValue("qtd", r.QuantidadePedidos);
                cmd.Parameters.AddWithValue("vtotal", r.TotalVendas);
                cmd.Parameters.AddWithValue("media", Math.Round(r.MediaVendas, 2));
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private async Task<List<MediaVendasClientes>> ConsultarMediaVendasClienteLocal()
        {
            var lista = new List<MediaVendasClientes>();
            using var conexao = new NpgsqlConnection(_stringConexao);
            await conexao.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "SELECT clienteid, nomecliente, quantidadepedidos, totalvendas, mediavendas FROM public.media_vendas_cliente ORDER BY totalvendas DESC", conexao);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                lista.Add(new MediaVendasClientes
                {
                    ClienteId = reader["clienteid"] == DBNull.Value ? Guid.Empty : Guid.Parse(reader["clienteid"].ToString()),
                    NomeCliente = reader["nomecliente"].ToString(),
                    QuantidadePedidos = Convert.ToInt32(reader["quantidadepedidos"]),
                    TotalVendas = Convert.ToDecimal(reader["totalvendas"]),
                    MediaVendas = Convert.ToDecimal(reader["mediavendas"])
                });
            }
            return lista;
        }

        private async Task SincronizarTotalVendas()
        {
            var pedidos = await _pedidoClient.ListarPedidosAsync();

            using var conexao = new NpgsqlConnection(_stringConexao);
            await conexao.OpenAsync();

            await using (var cmd = new NpgsqlCommand("DELETE FROM public.total_vendas", conexao))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            var totalPedidos = pedidos.Count;
            var totalVendas = pedidos.Sum(p => p.ValorTotal);

            await using var cmd2 = new NpgsqlCommand(
                @"INSERT INTO public.total_vendas (totalpedidos, totalvendas) VALUES (@tp, @tv)", conexao);
            cmd2.Parameters.AddWithValue("tp", totalPedidos);
            cmd2.Parameters.AddWithValue("tv", totalVendas);
            await cmd2.ExecuteNonQueryAsync();
        }

        private async Task<TotalVendas> ConsultarTotalVendasLocal()
        {
            using var conexao = new NpgsqlConnection(_stringConexao);
            await conexao.OpenAsync();

            await using var cmd = new NpgsqlCommand(
                "SELECT totalpedidos, totalvendas FROM public.total_vendas ORDER BY data_atualizacao DESC LIMIT 1", conexao);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new TotalVendas
                {
                    TotalPedidos = Convert.ToInt32(reader["totalpedidos"]),
                    ValorTotalVendas = Convert.ToDecimal(reader["totalvendas"])
                };
            }
            return new TotalVendas { TotalPedidos = 0, ValorTotalVendas = 0 };
        }

        List<EstatisticaVenda> IEstatisticaRepositorio.ObterTopProdutosVendidos(DateTime data, int top)
            => ObterTopProdutosVendidos(data, top).GetAwaiter().GetResult();

        int IEstatisticaRepositorio.ObterTotalNovosClientes(DateTime data)
            => ObterTotalNovosClientes(data).GetAwaiter().GetResult();

        decimal IEstatisticaRepositorio.ObterFaturamentoTotal(DateTime data)
            => ObterFaturamentoTotal(data).GetAwaiter().GetResult();

        List<MediaAvaliacaoProduto> IEstatisticaRepositorio.ObterMediaAvaliacaoProduto()
            => ObterMediaAvaliacaoProduto().GetAwaiter().GetResult();

        List<MediaVendaPorProduto> IEstatisticaRepositorio.ObterMediaVendaPorProduto()
            => ObterMediaVendaPorProduto().GetAwaiter().GetResult();

        List<MediaVendasClientes> IEstatisticaRepositorio.ObterMediaVendasClientes()
            => ObterMediaVendasClientes().GetAwaiter().GetResult();

        TotalVendas IEstatisticaRepositorio.ObterTotalVendas()
            => ObterTotalVendas().GetAwaiter().GetResult();
    }
}
