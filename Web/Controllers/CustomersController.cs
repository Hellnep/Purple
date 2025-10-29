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
    public async Task<ActionResult<Customer>> Post([FromBody] Customer inputData)
    {
        if (inputData is null)
            return BadRequest();

        inputData.Id = _purpleOcean.Customers.Count() + 1;
        inputData.Date = DateOnly.FromDateTime(DateTime.Now);

        _purpleOcean.Add(inputData);
        await _purpleOcean.SaveChangesAsync();

        return Ok(inputData);
    }

    [HttpPut("change")]
    public async Task<ActionResult<Customer>> Put([FromQuery]int id, [FromBody] Customer inputData)
    {
        try
        {
            Customer customer = _purpleOcean.Customers.First(customer => customer.Id == id);
            customer.Username = inputData.Username;

            await _purpleOcean.SaveChangesAsync();  
            return Ok(customer);
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }
    }
}
