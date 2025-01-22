using Application.Interfaces.IServices;
using Domain.Payload.File;
using Domain.Payload.Firma;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Metroli_PDF.Controllers
{
    [Route("api/firma")]
    [ApiController]
    public class FirmaController : ControllerBase
    {
        private readonly IFirmaService _firmaService;

        public FirmaController(IFirmaService firmaService)
        {
            _firmaService = firmaService;
        }


        [HttpPost("create")]
        public async Task<ActionResult> createFile([FromBody] CreateFirmaPayload payload) => Ok(await _firmaService.createFirma(payload));

        [HttpPut("update")]
        public async Task<ActionResult> updateFile([FromBody] UpdateFirmaPayload payload) => Ok(await _firmaService.updateFirma(payload));

        [HttpGet("listar")]
        public async Task<ActionResult> listarFile() => Ok(await _firmaService.listarFirma());
    }
}
