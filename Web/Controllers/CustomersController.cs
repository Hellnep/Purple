using Microsoft.AspNetCore.Mvc;

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
    public ActionResult<Customer> Get(long id)
    {
        try
        {
            Customer customer = _purpleOcean.Customers.FirstOrDefault(customer => customer.Id == id);

            if (customer is null)
                return NotFound("Customer not found");
            else
                return Ok(customer);
        }
        catch (Exception exception)
        {
            return StatusCode(500, exception.Message);
        }
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<CustomerDTO>> Post([FromBody] CustomerDTO inputData)
    {
        if (inputData.Username is null)
            return BadRequest();

        Customer customer = new()
        {
            Username = inputData.Username,
        };

        _purpleOcean.Add(customer);
        await _purpleOcean.SaveChangesAsync();

        return Ok(customer);
    }

    [HttpPut("change")]
    public async Task<ActionResult<CustomerDTO>> Put([FromQuery]int id, [FromBody] CustomerDTO inputData)
    {
        try
        {
            Customer customer = _purpleOcean.Customers.First(customer => customer.Id == id);
            customer.Username = inputData.Username ?? throw new ArgumentNullException();

            await _purpleOcean.SaveChangesAsync();  
            return Ok(customer);
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }
    }
}
