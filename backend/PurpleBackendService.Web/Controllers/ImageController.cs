using Microsoft.AspNetCore.Mvc;
using PurpleBackendService.Domain.Service;

namespace PurpleBackendService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpGet("{imagePath}", Name = nameof(GetImageByPath))]
        public async Task<IActionResult> GetImageByPath(string imagePath)
        {
            var result = await _imageService
                .GetImageFileAsync(imagePath);

            if (result.IsSuccess)
            {
                return File(result.Result.content, "image/jpeg");
            }
            else
            {
                return NotFound(result.Errors);
            }
        }
    }
}