using Microsoft.AspNetCore.Mvc;

using Purple.Web;
using Purple.Common.ModelValidator;
using Purple.Common.Database.Mapping;
using Purple.Common.Database.DTO.Sql;
using Purple.Common.Database.Entity.Sql;
using Purple.Common.Services.Interface;

namespace Purple.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private ICustomerService _service;

    public CustomersController(ICustomerService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<CustomerDTO>>> Get()
    {
        try
        {
            var result = await _service.GetCustomersAsync();

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
    public async Task<IActionResult> Get(long id)
    {
        try
        {
            var result = await _service.GetCustomerAsync(id);

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

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CustomerDTO input)
    {
        try
        {
            if (!Validate.TryValidate(input, out var results))
                return BadRequest(results);
            {
                var result = await _service.CreateCustomerAsync(input);

                if (result.IsSuccess)
                    return Ok(result.Data);
                else
                    return NotFound(result.Errors);
            }
        }
        catch (ArgumentNullException exception)
        {
            return StatusCode(500, exception.Message);
        }
    }
    
    [HttpPatch]
    public async Task<IActionResult> Patch([FromQuery] long id,
        [FromBody] CustomerDTO input)
    {
        try
        {
            if (!Validate.TryValidate(input, out var results))
                return BadRequest(results);
            else
            {
                var result = await _service.ChangeCustomerAsync(id, input);

                if (result.IsSuccess)
                    return Ok(result.Data);
                else
                    return NotFound(result.Errors);          
            }
        }
        catch (ArgumentNullException exception)
        {
            return StatusCode(500, exception.Message);
        }
    }
}
