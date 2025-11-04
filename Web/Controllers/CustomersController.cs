using Microsoft.AspNetCore.Mvc;

using Purple.Common.Database.Mapping;
using Purple.Common.Database.DTO.Sql;
using Purple.Common.Database.Entity.Sql;
using Purple.Common.Database.Context.Sqlite;

namespace Purple.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    private PurpleOcean _purpleOcean;

    public CustomersController(PurpleOcean purpleOcean)
    {
        _purpleOcean = purpleOcean;
    }

    [HttpGet]
    public ActionResult<List<Customer>> Get()
    {
        try
        {
            return _purpleOcean.Customers.ToList();
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpGet("{id}")]
    public IActionResult Get(long id)
    {
        try
        {
            var customer = _purpleOcean.Customers.FirstOrDefault(customer => customer.Id == id);

            if (customer is null)
                return NotFound("Customer not found");
            else
                return Ok(Mapping.Get<CustomerDTO, Customer>(customer));
        }
        catch (Exception exception)
        {
            return StatusCode(500, exception.Message);
        }
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Post([FromBody] CustomerDTO inputData)
    {
        if (inputData.Username is null)
            return BadRequest();

        Customer customer = Mapping.Get<Customer, CustomerDTO>(inputData);

        _purpleOcean.Add(customer);
        await _purpleOcean.SaveChangesAsync();

        return Ok(customer);
    }

    [HttpPut("change")]
    public async Task<IActionResult> Put([FromQuery] int id,
        [FromBody] CustomerDTO inputData)
    {
        try
        {
            var customer = _purpleOcean.Customers.FirstOrDefault(customer => customer.Id == id);

            if (customer is null)
                return NotFound();
            else
            {
                if (inputData.Username is not null)
                    customer.Username = inputData.Username;

                await _purpleOcean.SaveChangesAsync();  
                return Ok(customer);
            }            
        }
        catch (ArgumentNullException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}
