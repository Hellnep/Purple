using Microsoft.AspNetCore.Mvc;

using Purple.Common.Database.DTO.Sql;
using Purple.Common.ModelValidator;
using Purple.Common.Services.Interface;

namespace Purple.Web.Controllers;

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
        try
        {
            var result = await _service.GetProductsAsync();

            if (result.IsSuccess)
                return Ok(result.Data);
            else
                return NotFound(result.Errors);
        }
        catch (ArgumentNullException exception)
        {
            return StatusCode(500, exception.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDTO>> Get(long id)
    {
        try
        {
            var result = await _service.GetProductAsync(id);

            if (result.IsSuccess)
                return Ok(result.Data);
            else
                return NotFound(result.Errors);
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }    
    }

    [HttpGet]
    [Route("~/api/customers/{id}/[controller]")]
    public async Task<ActionResult<List<ProductDTO>>> GetAuthor(long id)
    {
        try
        {
            var result = await _service.GetAuthorProductsAsync(id);

            if (result.IsSuccess)
                return Ok(result.Data);
            else
                return NotFound(result.Errors);
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }    
    }

    [HttpPost]
    [Route("~/api/customers/{id}/[controller]")]
    public async Task<IActionResult> Post(long id, 
        [FromBody] ProductDTO input)
    {
        try
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
        catch (ArgumentNullException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPatch]
    [Route("~/api/customers/{customerId}/[controller]")]
    public async Task<IActionResult> Patch(long customerId, 
        [FromQuery] long id,
        [FromBody] ProductDTO input)
    {
        try
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
        catch (ArgumentNullException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}