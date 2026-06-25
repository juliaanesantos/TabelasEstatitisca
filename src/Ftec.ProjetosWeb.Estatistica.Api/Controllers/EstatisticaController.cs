using Ftec.ProjetosWeb.Estatistica.Aplicacao;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;

namespace Ftec.ProjetosWeb.Estatistica.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EstatisticaController : ControllerBase
    {
        private EstatisticaAplicacao _aplicacao;

        public EstatisticaController(IConfiguration config)
        {
            _aplicacao = new EstatisticaAplicacao(config["strConexao"]);
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
    }
}
