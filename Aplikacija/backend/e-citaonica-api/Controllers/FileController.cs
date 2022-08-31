using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace e_citaonica_api.Controllers
{
    [Route("files")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFileService _fileService;
        public FileController(
            IWebHostEnvironment webHostEnvironment, 
            IFileService fileService)
        {
            _webHostEnvironment = webHostEnvironment;
            _fileService = fileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            string newFilePath = await _fileService.SaveFile(file);

            return Ok(new { path = newFilePath });
        }

        [HttpPost("upload/image")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {   
            try
            {
                return Ok(new { path = await _fileService.SaveImage(file) });
            }
            catch (Exception ex)
            {
                return BadRequest(new { msg = ex.Message });
            }
        }
    }
}
