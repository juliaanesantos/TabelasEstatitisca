using Ftec.ProjetosWeb.Estatistica.Aplicacao;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace Ftec.ProjetosWeb.Estatistica.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MediaAvaliacaoProdutoController : ControllerBase
    {
        private readonly MediaAvaliacaoProdutoAplicacao aplicacao;

        public MediaAvaliacaoProdutoController(IConfiguration config)
        {
            aplicacao = new MediaAvaliacaoProdutoAplicacao(config["strConexao"]);
        }

        // GET api/MediaAvaliacaoProduto?nomeProduto=Nome&data=2024-01-01
        [HttpGet]
        public IActionResult Get([FromQuery] string nomeProduto, [FromQuery] string data)
        {
            try
            {
                if (string.IsNullOrEmpty(nomeProduto))
                    return BadRequest("Parâmetro 'nomeProduto' é obrigatório.");

                if (!DateTime.TryParse(data, out var dataParsed))
                    return BadRequest("Parâmetro 'data' inválido. Use formato yyyy-MM-dd ou outro reconhecível pelo DateTime.");

                var resultados = aplicacao.ListarAvaliacao(nomeProduto, dataParsed.Date);
                return Ok(resultados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/MediaAvaliacaoProduto/total?data=2024-01-01
        [HttpGet("total")]
        public IActionResult GetTotal([FromQuery] string data)
        {
            try
            {
                if (!DateTime.TryParse(data, out var dataParsed))
                    return BadRequest("Parâmetro 'data' inválido.");

                var total = aplicacao.ObterTotalAvalicoes(dataParsed.Date);
                return Ok(total);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // GET api/MediaAvaliacaoProduto/media?data=2024-01-01
        [HttpGet("media")]
        public IActionResult GetMedia([FromQuery] string data)
        {
            try
            {
                if (!DateTime.TryParse(data, out var dataParsed))
                    return BadRequest("Parâmetro 'data' inválido.");

                var media = aplicacao.ObterMediaAvaliacoes(dataParsed.Date);
                return Ok(media);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
