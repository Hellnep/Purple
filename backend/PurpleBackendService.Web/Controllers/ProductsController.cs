using Microsoft.AspNetCore.Mvc;

using PurpleBackendService.Web.Configure;
using PurpleBackendService.Web.Resource;
using PurpleBackendService.Domain.DTO;
using PurpleBackendService.Domain.Service;
using PurpleBackendService.Core.Utility;
using System.Threading.Tasks;

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

        [HttpGet(Name = nameof(GetProducts))]
        public async Task<ActionResult<List<ProductDTO>>> GetProducts()
        {
            var result = await _productService
                .GetProducts();

            if (result.IsSuccess)
            {
                var products = result.Result as List<ProductDTO>;
                List<Resource<ProductDTO>> resources = [];

                foreach (ProductDTO product in products!)
                {
                    Resource<ProductDTO> resource = new(product);

                    resource.AddLink("get", Url.Link(nameof(GetProducts), new { productId = product.Id })!);
                    resource.AddLink("patch",
                        Url.Link(nameof(PatchProduct), new
                            {
                                customerId = product.Author!.Id,
                                productId = product.Id
                            })!,
                        HttpMethod.Patch.Method
                    );
                }

                return Ok(resources);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpGet("{productId}", Name = nameof(GetProduct))]
        public async Task<ActionResult<ProductDTO>> GetProduct(long productId)
        {
            var result = await _productService
                .GetProduct(productId);

            if (result.IsSuccess)
            {
                ProductDTO product = result.Result!;

                long customerRefId = product.Author!.Id;
                var resource = new Resource<ProductDTO>(product);

                resource.AddLink("self", Url.Link(nameof(GetProduct), new { productId })!);
                resource.AddLink("patch", Url.Link(nameof(PatchProduct),
                    new { customerRefId, productId})!,
                    HttpMethod.Patch.Method
                );

                return Ok(resource);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpGet("~/api/customers/{customerId}/[controller]", Name = nameof(GetFromAuthor))]
        public async Task<ActionResult<List<ProductDTO>>> GetFromAuthor(long customerId)
        {
            var result = await _productService
                .GetAuthorProducts(customerId);

            if (result.IsSuccess)
            {
                var products = result.Result as List<ProductDTO>;
                List<Resource<ProductDTO>> resources = [];

                foreach (ProductDTO product in products!)
                {
                    Resource<ProductDTO> resource = new(product);

                    resource.AddLink("get", Url.Link(nameof(GetProducts), new { productId = product.Id })!);
                    resource.AddLink("patch",
                        Url.Link(nameof(PatchProduct), new
                            {
                                customerId = product.Author!.Id,
                                productId = product.Id
                            })!,
                        HttpMethod.Patch.Method
                    );
                }

                return Ok(resources);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        [HttpPost("~/api/customers/{customerId}/[controller]", Name = nameof(PostProduct))]
        public async Task<IActionResult> PostProduct(long customerId,
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
                    .CreateProductAsync(customerId, product);

                if (result.IsSuccess)
                {
                    var images = await _imageService
                        .AddImagesAsync(result.Result!.Id, files);

                    result.Result.Images = images.Result;

                    var resource = new Resource<ProductDTO>(result.Result!);
                    resource.AddLink("get", Url.Link(nameof(GetProduct),
                        new { productId = result.Result!.Id })!
                    );

                    resource.AddLink("patch", Url.Link(nameof(PatchProduct),
                        new { customerId, productId = result.Result!.Id})!
                    );

                    return Ok(resource);
                }
                else
                {
                    return NotFound(result.Errors);
                }
            }

            return BadRequest();
        }

        [HttpPatch("~/api/customers/{customerId}/[controller]/{productId}", Name = nameof(PatchProduct))]
        public async Task<IActionResult> PatchProduct(long customerId, long productId,
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