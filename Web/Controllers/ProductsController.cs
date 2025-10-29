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
        return _purpleOcean.Products.ToList();
    }

    [HttpGet("id={id}")]
    public ActionResult<Product> Get(long id)
    {
        Product product = _purpleOcean.Products.First(product => product.Id == id);

        if (product is null)
            return NotFound();
        else
            return product;
    }

    [HttpPost]
    [Route("create")]
    public async Task<ActionResult<Product>> Post([FromBody]Product product)
    {
        if (product is null)
            return NotFound();

        product.Id = _purpleOcean.Products.Count() + 1;

        _purpleOcean.Add(product);
        await _purpleOcean.SaveChangesAsync();

        return Ok(product);
    }
}