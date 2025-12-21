using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

using Purple.Web;
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
        public async Task<ActionResult<List<ProductDTO>>> Get()
        {
            var result = await _productService
                .GetProductsAsync();

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
        public async Task<ActionResult<ProductDTO>> Get(long productId)
        {
            var result = await _productService
                .GetProductAsync(productId);

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
        public async Task<ActionResult<List<ProductDTO>>> GetFromAuthor(long customerId)
        {
            var result = await _productService
                .GetAuthorProductsAsync(customerId);

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
            var product = new ProductDTO
            {
                Title = title,
                Content = content
            };

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
            var product = new ProductDTO
            {
                Title = title,
                Content = content
            };

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
    }
}