using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Purple.Web;
using Purple.Common.ModelValidator;
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
    public ActionResult<List<CustomerDTO>> Get()
    {
        try
        {
            List<CustomerDTO> customers = _purpleOcean.Customers
                .Select(customer => Mapping.Get<CustomerDTO, Customer>(customer))
                .ToList();
            
            return customers;
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(long id)
    {
        try
        {
            var customer = await _purpleOcean.Customers
                .FirstOrDefaultAsync(customer => customer.Id == id);

            if (customer is null)
                return NotFound($"Customer with ID {id} not found");
            else
                return Ok(Mapping.Get<CustomerDTO, Customer>(customer));
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] CustomerDTO inputData)
    {
        try
        {
            Customer customer = Mapping.Get<Customer, CustomerDTO>(inputData);

            if (!Validate.TryValidate(customer, out var results))
                this.ValidationProblems(results);
            else
                _purpleOcean.Add(customer);

            await _purpleOcean.SaveChangesAsync();

            CustomerDTO customerDto =  Mapping.Get<CustomerDTO, Customer>(customer);
            return CreatedAtAction(
                nameof(Get),
                new { Id = customer.Id },
                customerDto
            );
        }
        catch (ArgumentNullException exception)
        {
            return StatusCode(500, exception.Message);
        }
    }

    [HttpPatch]
    public async Task<IActionResult> Patch([FromQuery] long id,
        [FromBody] CustomerDTO inputData)
    {
        try
        {
            var customer = _purpleOcean.Customers
                .FirstOrDefault(customer => customer.Id == id);

            if (customer is null)
                return NoContent();
            else
            {
                if (!Validate.TryValidate(inputData, out var results))
                    this.ValidationProblems(results);

                customer.FirstName = inputData.FirstName is not null
                    ? inputData.FirstName
                    : customer.FirstName;

                customer.Email = inputData.Email is not null
                    ? inputData.Email
                    : customer.Email;

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
