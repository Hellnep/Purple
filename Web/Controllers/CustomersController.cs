using Microsoft.AspNetCore.Mvc;

using Purple.Common.Database.Entity.Sqlite;
using Purple.Common.Database.Context.Sqlite;
using Microsoft.EntityFrameworkCore;

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
    public ActionResult<Customer> Post([FromBody]Customer customer)
    {
        if (customer is null)
            return BadRequest();

        customer.Id = _purpleOcean.Customers.Count() + 1;
        customer.Date = DateOnly.FromDateTime(DateTime.Now);

        _purpleOcean.Add(customer);
        _purpleOcean.SaveChanges();

        return Ok(customer);
    }
}
