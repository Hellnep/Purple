using Microsoft.AspNetCore.Mvc;

using PurpleBackendService.Domain.DTO;
using PurpleBackendService.Domain.Service;
using PurpleBackendService.Domain.Utility;

namespace PurpleBackendService.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private IProductService _productService;

        public ProductsController(IProductService service)
        {
            _productService = service;
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
            [FromBody] ProductDTO product)
        {
            if (!Validate.TryValidate(product, out var results))
            {
                return BadRequest(results);
            }
            else
            {
                var result = await _productService
                    .CreateProductAsync(custometId, product);

                if (result.IsSuccess)
                {
                    return Ok(result.Result);
                }
                else
                {
                    return NotFound(result.Errors);
                }
            }
        }

        [HttpPatch]
        [Route("~/api/customers/{customerId}/[controller]")]
        public async Task<IActionResult> Patch(long customerId, 
            [FromQuery] long productId, 
            [FromBody] ProductDTO product
        )
        {
            if (!Validate.TryValidate(product, out var results))
            {
                return BadRequest(results);
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
        }
    }
}