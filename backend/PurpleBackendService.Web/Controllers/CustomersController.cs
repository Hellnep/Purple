using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using PurpleBackendService.Domain.DTO;
using PurpleBackendService.Domain.Service;
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
        var result = await _service.GetCustomersAsync();

        if (result.IsSuccess)
            return Ok(result.Data);
        else
            return NotFound(result.Errors);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> Get(long id)
    {
        var result = await _service.GetCustomerAsync(id);

        if (result.IsSuccess)
            return Ok(result.Data);
        else
            return NotFound(result.Errors);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromBody] CustomerDTO input)
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
    
    [HttpPatch]
    public async Task<ActionResult> Patch([FromQuery] long id,
        [FromBody] CustomerDTO input)
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
}
