using Microsoft.AspNetCore.Mvc;

using Purple.Common.Database.Mapping;
using Purple.Common.Database.DTO.Sql;
using Purple.Common.Database.Entity.Sql;
using Purple.Common.Database.Context.Sqlite;

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
    public IActionResult Get(long id)
    {
        try
        {
            var product = _purpleOcean.Products.FirstOrDefault(product => product.Id == id);

            if (product is null)
                return NotFound();
            else
                return Ok(product);
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }    
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> Post([FromBody] ProductDTO inputData)
    {
        if (inputData is null)
            return BadRequest();

        Product product = Mapping.Get<Product, ProductDTO>(inputData);

        _purpleOcean.Add(product);
        await _purpleOcean.SaveChangesAsync();

        return Ok(product);
    }

    [HttpPut("change")]
    public async Task<IActionResult> Put([FromQuery] int id,
        [FromBody] ProductDTO inputData)
    {
        try
        {
            var product = _purpleOcean.Products.FirstOrDefault(product => product.Id == id);

            if (product is null)
                return NotFound();
            else
            {
                if (inputData.Name is not null)
                    product.Name = inputData.Name;

                product.Description = inputData.Description;

                await _purpleOcean.SaveChangesAsync();
                return Ok(product);
            }
        }
        catch (ArgumentNullException exception)
        {
            return BadRequest(exception.Message);
        }
    }
}