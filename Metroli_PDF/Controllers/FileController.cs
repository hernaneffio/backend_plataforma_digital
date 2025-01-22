using Application.Interfaces.IServices;
using Domain.Payload.File;
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
        public async Task<ActionResult> createFile([FromBody] UpdateFilePayload payload) => Ok(await _fileService.createFile(payload));

        //[HttpPut("update")]
        //public async Task<ActionResult> updateFile([FromBody] UpdateFilePayload payload) => Ok(await _fileService.updateFile(payload));

        //[HttpGet("listar")]
        //public async Task<ActionResult> listarFile([FromBody] UpdateFilePayload payload) => Ok(await _fileService.listarFile(payload));
    }
}
