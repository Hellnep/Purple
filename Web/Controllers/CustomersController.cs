using Microsoft.AspNetCore.Mvc;

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

    [HttpGet("id={id}")]
    public ActionResult<Customer> Get(int id)
    {
        try
        {
            return _purpleOcean.Customers.Single(product => product.Id == id);
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }     
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<Customer>> Post([FromBody] Customer data)
    {
        if (data is null)
            return BadRequest();

        data.Id = _purpleOcean.Customers.Count() + 1;
        data.Date = DateOnly.FromDateTime(DateTime.Now);

        _purpleOcean.Add(data);
        await _purpleOcean.SaveChangesAsync();

        return Ok(data);
    }

    [HttpPut("{id}/change")]
    public async Task<ActionResult<Customer>> Put(int id, [FromBody] Customer data)
    {
        try
        {
            Customer customer = _purpleOcean.Customers.First(customer => customer.Id == id);
            customer.Username = data.Username;

            await _purpleOcean.SaveChangesAsync();  
            return Ok(customer);
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }
    }
}
