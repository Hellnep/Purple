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
    public async Task<ActionResult<ProductDTO>> Post([FromBody] ProductDTO inputData)
    {
        if (inputData is null)
            return BadRequest();

        Product product = Mapping.Get<Product, ProductDTO>(inputData);

        _purpleOcean.Add(product);
        await _purpleOcean.SaveChangesAsync();

        return Ok(product);
    }

    [HttpPut("change")]
    public async Task<ActionResult<ProductDTO>> Put([FromQuery] int id, [FromBody] ProductDTO inputData)
    {
        try
        {
            Product product = _purpleOcean.Products.First(product => product.Id == id);

            product.Name = inputData.Name ?? throw new ArgumentNullException();
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