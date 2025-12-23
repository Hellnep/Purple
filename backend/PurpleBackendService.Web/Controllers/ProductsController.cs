using Microsoft.AspNetCore.Mvc;

using PurpleBackendService.Web.Configure;
using PurpleBackendService.Domain.DTO;
using PurpleBackendService.Domain.Service;
using PurpleBackendService.Core.Utility;

namespace PurpleBackendService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        private readonly IImageService _imageService;


        public ProductsController(IProductService service, IImageService imageService)
        {
            _productService = service;
            _imageService = imageService;
        }

        [HttpGet]
        public ActionResult<List<ProductDTO>> Get()
        {
            var result = _productService
                .GetProducts();

            if (result.IsSuccess)
            {
                return Ok(result.Result);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpGet("{productId}")]
        public ActionResult<ProductDTO> Get(long productId)
        {
            var result = _productService
                .GetProduct(productId);

            if (result.IsSuccess)
            {
                return Ok(result.Result);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpGet]
        [Route("~/api/customers/{customerId}/[controller]")]
        public ActionResult<List<ProductDTO>> GetFromAuthor(long customerId)
        {
            var result = _productService
                .GetAuthorProducts(customerId);

            if (result.IsSuccess)
            {
                return Ok(result.Result);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpPost]
        [Route("~/api/customers/{custometId}/[controller]")]
        public async Task<IActionResult> Post(long custometId,
            [FromForm] string title,
            [FromForm] string content,
            [FromForm] IFormFileCollection files
        )
        {
            var product = Create(title, content);

            if (!Validate.TryValidate(product, out var results))
            {
                this.ValidationProblems(results);
            }
            else
            {
                var result = await _productService
                    .CreateProductAsync(custometId, product);

                if (result.IsSuccess)
                {
                    var images = await _imageService
                        .AddImagesAsync(result.Result!.Id, files);

                    result.Result.Images = images.Result;

                    return Ok(result.Result);
                }
                else
                {
                    return NotFound(result.Errors);
                }
            }

            return BadRequest();
        }

        [HttpPatch]
        [Route("~/api/customers/{customerId}/[controller]")]
        public async Task<IActionResult> Patch(long customerId,
            [FromQuery] long productId,
            [FromForm] string title,
            [FromForm] string content
        )
        {
            var product = Create(title, content);

            if (!Validate.TryValidate(product, out var results))
            {
                this.ValidationProblems(results);
            }
            else
            {
                var result = await _productService
                    .ChangeProductAsync(customerId, productId, product);

                if (result.IsSuccess)
                {
                    return Ok(result.Result);
                }
                else
                {
                    return NotFound(result.Errors);
                }
            }

            return BadRequest();
        }

        private static ProductDTO Create(string? title, string? content) =>
            new()
            {
                Title = title,
                Content = content
            };
    }
}