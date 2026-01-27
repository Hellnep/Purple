using Microsoft.AspNetCore.Mvc;

using PurpleBackendService.Web.Configure;
using PurpleBackendService.Web.Resource;
using PurpleBackendService.Web.Models.DTOs.Image;
using PurpleBackendService.Web.Models.DTOs.Product;
using PurpleBackendService.Core.Utility;
using PurpleBackendService.Core.Interfaces.Services;
using PurpleBackendService.Domain.Entity;

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
                var products = result.Result;
                var resources = new ResourceCollection<ResourceObject<ProductDTO>>([]);

                foreach (var product in products!)
                {
                    var productDTO = Mapping.Get<ProductDTO, Product>(product);
                    var images = product.Images as List<ImageDTO> ?? [];

                    ResourceObject<ProductDTO> resource = new(productDTO);

                    resource.AddLink("patch",
                        Url.Link(nameof(PatchProduct), new
                            {
                                userId = productDTO.Author!.Id,
                                productId = productDTO.Id
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

                    resources.Items.Add(resource);
                }

                resources.AddLink("self",
                    Url.Link(nameof(GetProducts), null)!
                );

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
                var product = Mapping.Get<ProductDTO, Product>(result.Result!);

                var userRefId = product.Author!.Id;
                var resource = new ResourceObject<ProductDTO>(product);

                var images = product.Images as List<ImageDTO>
                    ?? new List<ImageDTO>();

                resource.AddLink("self",
                    Url.Link(nameof(GetProduct), new { productId })!
                );

                resource.AddLink("patch",
                    Url.Link(nameof(PatchProduct), new { userRefId, productId})!,
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
                var resources = new ResourceCollection<ResourceObject<ProductDTO>>([]);

                foreach (var product in result.Result!)
                {
                    var productDTO = Mapping.Get<ProductDTO, Product>(product);
                    ResourceObject<ProductDTO> resource = new(productDTO);

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

                    resources.Items.Add(resource);
                }

                resources.AddLink("self",
                    Url.Link(nameof(GetFromAuthor), new { userId })!
                );

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
            var createdProduct = Create(title, content);

            if (!Validate.TryValidate(createdProduct, out var results))
            {
                this.ValidationProblems(results);
            }
            else
            {
                var result = await _productService
                    .CreateProductAsync(userId,
                        Mapping.Get<Product, ProductDTO>(createdProduct)
                    );

                if (result.IsSuccess)
                {
                    var product = Mapping.Get<ProductDTO, Product>(result.Result!);

                    var images = await _imageService
                        .AddImagesAsync(result.Result!.Id, files);

                    result.Result.Images = images.Result;

                    var resource = new ResourceObject<ProductDTO>(product);

                    resource.AddLink("self",
                        Url.Link(nameof(GetProduct), new { userId = userId, })!,
                        HttpMethod.Post.Method
                    );

                    if (images.Result is not null && images.Result.Count > 0)
                    {
                        foreach (var image in images.Result)
                        {
                            resource.AddLink($"get-{image.Title}",
                                Url.Link("GetImageByPath", new { imagePath = image.Path })!
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
            var changedProduct = Create(title, content);

            if (!Validate.TryValidate(changedProduct, out var results))
            {
                this.ValidationProblems(results);
            }
            else
            {
                var result = await _productService
                    .ChangeProductAsync(productId,
                        Mapping.Get<Product, ProductDTO>(changedProduct)
                    );

                if (result.IsSuccess)
                {
                    var product = Mapping.Get<ProductDTO, Product>(result.Result!);

                    var resource = new ResourceObject<ProductDTO>(product);

                    resource.AddLink("self",
                        Url.Link(nameof(PatchProduct), new { productId })!,
                        HttpMethod.Patch.Method
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
