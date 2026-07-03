using Ftec.ProjetosWeb.Estatistica.Dominio.Entidades;
using Ftec.ProjetosWeb.Estatistica.Dominio.Interfaces;
using Ftec.ProjetosWeb.Estatistica.Persistencia.ApiClientes;
using Npgsql;

namespace Ftec.ProjetosWeb.Estatistica.Persistencia
{
    public class MediaAvaliacaoProdutoRepositorio : IMediaAvaliacaoProdutoRepositorio
    {
        private readonly string _stringConexao;
        private readonly AvaliacaoApiClient _avaliacaoClient;
        private readonly ProdutoApiClient _produtoClient;

        public MediaAvaliacaoProdutoRepositorio(
            string stringConexao,
            AvaliacaoApiClient avaliacaoClient,
            ProdutoApiClient produtoClient)
        {
            _stringConexao = stringConexao;
            _avaliacaoClient = avaliacaoClient;
            _produtoClient = produtoClient;
        }

        public List<MediaAvaliacaoProduto> ObterAvaliacoesProdutos(string nomeProduto, DateTime data)
        {
            SincronizarAvaliacoesAsync(nomeProduto, data).GetAwaiter().GetResult();
            return ConsultarAvaliacoesLocais(nomeProduto, data);
        }

        public int ObterTotalAvalicoes(DateTime data)
        {
            SincronizarAvaliacoesAsync(null, data).GetAwaiter().GetResult();
            return ConsultarTotalAvaliacoesLocal(data);
        }

        public decimal ObterMediaAvaliacoes(DateTime data)
        {
            SincronizarAvaliacoesAsync(null, data).GetAwaiter().GetResult();
            return ConsultarMediaAvaliacoesLocal(data);
        }

        private async Task SincronizarAvaliacoesAsync(string nomeProduto, DateTime data)
        {
            var produtos = await _produtoClient.ListarProdutosAsync();
            var produtosMap = new Dictionary<Guid, string>();
            foreach (var p in produtos.Where(p => !string.IsNullOrEmpty(p?.Id)))
            {
                if (Guid.TryParse(p.Id, out var pid))
                    produtosMap[pid] = p.Nome;
            }

            List<AvaliacaoResponse> avaliacoes;

            if (!string.IsNullOrEmpty(nomeProduto))
            {
                var produto = produtos.FirstOrDefault(p => p.Nome?.Equals(nomeProduto, StringComparison.OrdinalIgnoreCase) == true);
                if (produto != null && !string.IsNullOrEmpty(produto.Id) && Guid.TryParse(produto.Id, out var pid))
                    avaliacoes = await _avaliacaoClient.ListarAvaliacoesPorProdutoAsync(pid);
                else
                    avaliacoes = new List<AvaliacaoResponse>();
            }
            else
            {
                avaliacoes = await _avaliacaoClient.ListarTodasAvaliacoesAsync(produtos);
            }

            using var conexao = new NpgsqlConnection(_stringConexao);
            await conexao.OpenAsync();

            await using (var cmd = new NpgsqlCommand("DELETE FROM public.avaliacoes_diarias WHERE data_avaliacao = @data", conexao))
            {
                cmd.Parameters.AddWithValue("data", data.Date);
                await cmd.ExecuteNonQueryAsync();
            }

            var avaliacoesFiltradas = avaliacoes
                .Where(a => a.DataAvaliacao.Date == data.Date && !string.IsNullOrEmpty(a.ProdutoId))
                .ToList();

            var grupos = avaliacoesFiltradas
                .Where(a => Guid.TryParse(a.ProdutoId, out _))
                .GroupBy(a => a.ProdutoId)
                .Select(g =>
                {
                    var pid = Guid.Parse(g.Key);
                    return new
                    {
                        NomeProduto = produtosMap.ContainsKey(pid) ? produtosMap[pid] : (nomeProduto ?? "Desconhecido"),
                        Data = data.Date,
                        Quantidade = g.Count(),
                        Soma = g.Sum(a => a.Avaliacao),
                        Media = Math.Round(g.Average(a => a.Avaliacao), 2)
                    };
                });

            foreach (var g in grupos)
            {
                await using var cmd = new NpgsqlCommand(
                    @"INSERT INTO public.avaliacoes_diarias (nomeproduto, data_avaliacao, quantidade, soma, media)
                      VALUES (@nome, @data, @qtd, @soma, @media)", conexao);
                cmd.Parameters.AddWithValue("nome", g.NomeProduto);
                cmd.Parameters.AddWithValue("data", g.Data);
                cmd.Parameters.AddWithValue("qtd", g.Quantidade);
                cmd.Parameters.AddWithValue("soma", g.Soma);
                cmd.Parameters.AddWithValue("media", g.Media);
                await cmd.ExecuteNonQueryAsync();
            }
        }

        private List<MediaAvaliacaoProduto> ConsultarAvaliacoesLocais(string nomeProduto, DateTime data)
        {
            var lista = new List<MediaAvaliacaoProduto>();
            using var conexao = new NpgsqlConnection(_stringConexao);
            conexao.Open();

            using var cmd = new NpgsqlCommand(
                @"SELECT nomeproduto, data_avaliacao, quantidade, soma, media
                  FROM public.avaliacoes_diarias
                  WHERE data_avaliacao = @data AND nomeproduto = @nome", conexao);
            cmd.Parameters.AddWithValue("data", data.Date);
            cmd.Parameters.AddWithValue("nome", nomeProduto);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new MediaAvaliacaoProduto
                {
                    NomeProduto = reader["nomeproduto"].ToString(),
                    Data = Convert.ToDateTime(reader["data_avaliacao"]),
                    QuantidadeAvaliacao = Convert.ToInt32(reader["quantidade"]),
                    SomaAvaliacao = Convert.ToInt32(reader["soma"]),
                    MediaAvaliacao = Convert.ToDecimal(reader["media"]),
                    TotalAvaliacao = Convert.ToInt32(reader["quantidade"]),
                    TotalAvaliacoes = Convert.ToInt32(reader["quantidade"])
                });
            }
            return lista;
        }

        private int ConsultarTotalAvaliacoesLocal(DateTime data)
        {
            using var conexao = new NpgsqlConnection(_stringConexao);
            conexao.Open();

            using var cmd = new NpgsqlCommand(
                "SELECT COALESCE(SUM(quantidade), 0) FROM public.avaliacoes_diarias WHERE data_avaliacao = @data", conexao);
            cmd.Parameters.AddWithValue("data", data.Date);
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? 0 : Convert.ToInt32(result);
        }

        private decimal ConsultarMediaAvaliacoesLocal(DateTime data)
        {
            using var conexao = new NpgsqlConnection(_stringConexao);
            conexao.Open();

            using var cmd = new NpgsqlCommand(
                "SELECT COALESCE(AVG(media), 0) FROM public.avaliacoes_diarias WHERE data_avaliacao = @data", conexao);
            cmd.Parameters.AddWithValue("data", data.Date);
            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? 0 : Convert.ToDecimal(result);
        }
    }
}
