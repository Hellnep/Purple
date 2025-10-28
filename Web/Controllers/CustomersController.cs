using Microsoft.AspNetCore.Mvc;

using Purple.Common.Database.Entity.Sql;
using Purple.Common.Database.Context.Sqlite;

namespace Purple.Web.Controllers;

[ApiController]
[Route("[controller]")]
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
        return _purpleOcean.Customers.ToList();
    }

    [HttpGet("id={id}")]
    public ActionResult<Customer> Get(int id)
    {
        return _purpleOcean.Customers.Single(product => product.Id == id);
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<Customer>> Post([FromBody]Customer customer)
    {
        if (customer is null)
            return BadRequest();

        customer.Id = _purpleOcean.Customers.Count() + 1;
        customer.Date = DateOnly.FromDateTime(DateTime.Now);

        _purpleOcean.Add(customer);
        await _purpleOcean.SaveChangesAsync();

        return Ok(customer);
    }
}
