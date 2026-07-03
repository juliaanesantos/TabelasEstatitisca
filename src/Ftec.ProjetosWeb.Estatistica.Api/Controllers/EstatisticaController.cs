using Ftec.ProjetosWeb.Estatistica.Aplicacao;
using Microsoft.AspNetCore.Mvc;

namespace Ftec.ProjetosWeb.Estatistica.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstatisticaController : ControllerBase
    {
        private readonly EstatisticaAplicacao _aplicacao;

        public EstatisticaController(EstatisticaAplicacao aplicacao)
        {
            _aplicacao = aplicacao;
        }

        [HttpGet("painel-hoje")]
        public IActionResult GetPainel()
        {
            try
            {
                var dados = _aplicacao.GerarPainelDiario(DateTime.Now);
                return Ok(dados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("media-avaliacao-produto")]
        public IActionResult GetMediaAvaliacaoProduto()
        {
            try
            {
                var dados = _aplicacao.ObterMediaAvaliacaoProduto();
                return Ok(dados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("media-venda-produto")]
        public IActionResult GetMediaVendaPorProduto()
        {
            try
            {
                var dados = _aplicacao.ObterMediaVendaPorProduto();
                return Ok(dados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("media-vendas-cliente")]
        public IActionResult GetMediaVendasClientes()
        {
            try
            {
                var dados = _aplicacao.ObterMediaVendasClientes();
                return Ok(dados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("total-vendas")]
        public IActionResult GetTotalVendas()
        {
            try
            {
                var dados = _aplicacao.ObterTotalVendas();
                return Ok(dados);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
