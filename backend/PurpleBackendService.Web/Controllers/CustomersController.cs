using Microsoft.AspNetCore.Mvc;
using PurpleBackendService.Core;
using PurpleBackendService.Domain.DTO;
using PurpleBackendService.Domain.Utility;

namespace PurpleBackendService.Web.Controllers;

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
    public async Task<ActionResult<CustomerDTO>> Get(long id)
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
    public async Task<ActionResult> Post([FromBody] CustomerDTO input)
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
    public async Task<ActionResult> Patch([FromQuery] long id,
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
