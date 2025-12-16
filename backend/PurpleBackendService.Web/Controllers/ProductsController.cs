using Microsoft.AspNetCore.Mvc;

using PurpleBackendService.Domain.DTO;
using PurpleBackendService.Domain.Service;
using PurpleBackendService.Domain.Utility;

namespace PurpleBackendService.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private IProductService _service;

    public ProductsController(IProductService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<ProductDTO>>> Get()
    {
        var result = await _service.GetProductsAsync();

        if (result.IsSuccess)
            return Ok(result.Data);
        else
            return NotFound(result.Errors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDTO>> Get(long id)
    {
        var result = await _service.GetProductAsync(id);

        if (result.IsSuccess)
            return Ok(result.Data);
        else
            return NotFound(result.Errors);  
    }

    [HttpGet]
    [Route("~/api/customers/{id}/[controller]")]
    public async Task<ActionResult<List<ProductDTO>>> GetAuthor(long id)
    {
        var result = await _service.GetAuthorProductsAsync(id);

        if (result.IsSuccess)
            return Ok(result.Data);
        else
            return NotFound(result.Errors);  
    }

    [HttpPost]
    [Route("~/api/customers/{id}/[controller]")]
    public async Task<IActionResult> Post(long id, 
        [FromBody] ProductDTO input)
    {
        if (!Validate.TryValidate(input, out var results))
            return BadRequest(results);
        else
        {
            var result = await _service.CreateProductAsync(id, input);

            if (result.IsSuccess)
                return Ok(result.Data);
            else
                return NotFound(result.Errors);
        }
    }

    [HttpPatch]
    [Route("~/api/customers/{customerId}/[controller]")]
    public async Task<IActionResult> Patch(long customerId, 
        [FromQuery] long id,
        [FromBody] ProductDTO input)
    {
        if (!Validate.TryValidate(input, out var results))
            return BadRequest(results);
        else
        {
            var result = await _service.ChangeProductAsync(customerId, id, input);

            if (result.IsSuccess)
                return Ok(result.Data);
            else
                return NotFound(result.Errors);          
        }
    }
}