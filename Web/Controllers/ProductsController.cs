using Microsoft.AspNetCore.Mvc;

using Purple.Common.Database.Context.Sqlite;
using Purple.Common.Database.Entity.Sql;

namespace Purple.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private PurpleOcean _purpleOcean;

    public ProductsController(PurpleOcean purpleOcean)
    {
        _purpleOcean = purpleOcean;
    }

    [HttpGet]
    public ActionResult<List<Product>> Get()
    {
        try
        {
            return _purpleOcean.Products.ToList();
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<Product> Get(long id)
    {
        try
        {
            Product product = _purpleOcean.Products.First(product => product.Id == id);
            return product;
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }    
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<Product>> Post([FromBody] Product inputData)
    {
        if (inputData is null)
            return BadRequest();

        inputData.Id = _purpleOcean.Products.Count() + 1;

        _purpleOcean.Add(inputData);
        await _purpleOcean.SaveChangesAsync();

        return Ok(inputData);
    }

    [HttpPut("change")]
    public async Task<ActionResult<Product>> Put([FromQuery] int id, [FromBody] Product inputData)
    {
        try
        {
            Product product = _purpleOcean.Products.First(product => product.Id == id);

            product.Name = inputData.Name;
            product.Description = inputData.Description;

            await _purpleOcean.SaveChangesAsync();
            return Ok(product);
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }
    }
}