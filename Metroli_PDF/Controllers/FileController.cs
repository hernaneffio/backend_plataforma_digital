using Application.Interfaces.IServices;
using Application.Services;
using Domain.Payload.File;
using Domain.Payload.Firma;
using Microsoft.AspNetCore.Mvc;

namespace Metroli_PDF.Controllers
{
    [Route("api/file")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("create")]
        public async Task<ActionResult> createFile([FromBody] CreateFilePayload payload) => Ok(await _fileService.createFile(payload));

        [HttpPost("create-new")]
        public async Task<ActionResult> createFileNew([FromBody] CreateFileNewPayload payload) => Ok(await _fileService.createFileNew(payload));

        [HttpDelete("delete")]
        public async Task<ActionResult> deleteFile([FromBody] DeleteFirmaPayload payload) => Ok(await _fileService.deleteFile(payload));

        [HttpGet("listar")]
        public async Task<ActionResult> listarFile([FromQuery] string? archivo) => Ok(await _fileService.listarFile(archivo));

        //[HttpPut("update")]
        //public async Task<ActionResult> updateFile([FromBody] UpdateFilePayload payload) => Ok(await _fileService.updateFile(payload));
    }
}
