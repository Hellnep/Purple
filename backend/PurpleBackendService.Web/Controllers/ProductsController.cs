using Microsoft.AspNetCore.Mvc;

using PurpleBackendService.Web.Configure;
using PurpleBackendService.Web.Resource;
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

        /// <summary>
        /// Get all products from database
        /// </summary>
        /// <returns>Returns a list of products</returns>
        [HttpGet(Name = nameof(GetProducts))]
        public async Task<IActionResult> GetProducts()
        {
            var result = await _productService
                .GetProductsAsync();

            if (result.IsSuccess)
            {
                var products = result.Result as List<ProductDTO>;
                List<Resource<ProductDTO>> resources = [];

                foreach (var product in products!)
                {
                    Resource<ProductDTO> resource = new(product);
                    var images = product.Images as List<ImageDTO>
                        ?? new List<ImageDTO>();

                    resource.AddLink($"patch",
                        Url.Link(nameof(PatchProduct), new
                            {
                                userId = product.Author!.Id,
                                productId = product.Id
                            })!,
                        HttpMethod.Patch.Method
                    );

                    if (images.Count > 0 && images is not null)
                    {
                        foreach (var image in images)
                        {
                            resource.AddLink($"get-{image.Title}",
                                Url.Link("GetImageByPath", new
                                {
                                    imagePath = image.Path
                                })!
                            );
                        }
                    }

                    resources.Add(resource);
                }

                return Ok(resources);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        /// <summary>
        /// Receive a product by Id from database
        /// </summary>
        /// <param name="productId">A product identificator</param>
        /// <returns>Returns the product with the id</returns>
        [HttpGet("{productId}", Name = nameof(GetProduct))]
        public async Task<ActionResult<ProductDTO>> GetProduct(long productId)
        {
            var result = await _productService
                .GetProductAsync(productId);

            if (result.IsSuccess)
            {
                var product = result.Result!;

                var userRefId = product.Author!.Id;
                var resource = new Resource<ProductDTO>(product);

                var images = product.Images as List<ImageDTO>
                    ?? new List<ImageDTO>();

                resource.AddLink("patch", Url.Link(nameof(PatchProduct),
                    new { userRefId, productId})!,
                    HttpMethod.Patch.Method
                );

                if (images.Count > 0 && images is not null)
                {
                    foreach (var image in images)
                    {
                        resource.AddLink($"get-{image.Title}",
                            Url.Link("GetImageByPath", new
                            {
                                imagePath = image.RelativePath!.Split('/')[3]
                            })!
                        );
                    }
                }

                return Ok(resource);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        /// <summary>
        /// Receive product from author
        /// </summary>
        /// <param name="userId">A user identificator</param>
        /// <returns>Returns the user's product</returns>
        [HttpGet("~/api/users/{userId}/[controller]", Name = nameof(GetFromAuthor))]
        public async Task<ActionResult<List<ProductDTO>>> GetFromAuthor(long userId)
        {
            var result = await _productService
                .GetAuthorProductsAsync(userId);

            if (result.IsSuccess)
            {
                var products = result.Result as List<ProductDTO>;
                List<Resource<ProductDTO>> resources = [];

                foreach (var product in products!)
                {
                    Resource<ProductDTO> resource = new(product);

                    resource.AddLink("patch",
                        Url.Link(nameof(PatchProduct), new
                            {
                                userId = product.Author!.Id,
                                productId = product.Id
                            })!,
                        HttpMethod.Patch.Method
                    );

                    var images = product.Images as List<ImageDTO>
                    ?? new List<ImageDTO>();

                    if (images.Count > 0 && images is not null)
                    {
                        foreach (var image in images)
                        {
                            resource.AddLink($"get-{image.Title}",
                                Url.Link("GetImageByPath", new
                                {
                                    imagePath = image.RelativePath!.Split('/')[3]
                                })!
                            );
                        }
                    }

                    resources.Add(resource);
                }

                return Ok(resources);
            }
            else
            {
                return NotFound(result.Errors);
            }
        }

        /// <summary>
        /// Creating a product with
        /// subsequent placement in
        /// the database
        /// </summary>
        /// <param name="userId">A user identificator</param>
        /// <param name="title">Product title</param>
        /// <param name="content">Description</param>
        /// <param name="files">Image files</param>
        /// <returns>Receive a created product</returns>
        [HttpPost("~/api/users/{userId}/[controller]", Name = nameof(PostProduct))]
        public async Task<IActionResult> PostProduct(long userId,
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
                    .CreateProductAsync(userId, product);

                if (result.IsSuccess)
                {
                    var images = await _imageService
                        .AddImagesAsync(result.Result!.Id, files);

                    result.Result.Images = images.Result;

                    var resource = new Resource<ProductDTO>(result.Result!);

                    if (images.Result is not null && images.Result.Count > 0)
                    {
                        foreach (var image in images.Result)
                        {
                            resource.AddLink($"get-{image.Title}",
                                Url.Link("GetImageByPath", new
                                {
                                    imagePath = image.RelativePath!.Split('/')[3]
                                })!
                            );
                        }
                    }

                    return Ok(resource);
                }
                else
                {
                    return NotFound(result.Errors);
                }
            }

            return BadRequest();
        }

        /// <summary>
        /// Changes the data of a product existing
        /// in the database
        /// </summary>
        /// <param name="productId">Product identificator</param>
        /// <param name="title">Product title</param>
        /// <param name="content">Description</param>
        /// <returns>Returns a changed product</returns>
        [HttpPatch("{productId}", Name = nameof(PatchProduct))]
        public async Task<IActionResult> PatchProduct(long productId,
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
                    .ChangeProductAsync(productId, product);

                if (result.IsSuccess)
                {
                    var resource = new Resource<ProductDTO>(result.Result!);
                    resource.AddLink("get", Url.Link(nameof(GetProduct),
                        new { productId })!
                    );

                    var images = result.Result!.Images as List<ImageDTO>
                        ?? new List<ImageDTO>();

                    if (images is not null && images.Count > 0)
                    {
                        foreach (var image in images)
                        {
                            resource.AddLink($"get-{image.Title}",
                                Url.Link("GetImageByPath", new
                                {
                                    imagePath = image.RelativePath!.Split('/')[3]
                                })!
                            );
                        }
                    }

                    return Ok(resource);
                }
                else
                {
                    return NotFound(result.Errors);
                }
            }

            return BadRequest();
        }

        private static ProductDTO Create(string? title, string? content) => new()
        {
            Title = title,
            Content = content
        };
    }
}
