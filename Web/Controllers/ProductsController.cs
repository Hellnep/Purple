using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Purple.Common.Database.Mapping;
using Purple.Common.Database.DTO.Sql;
using Purple.Common.Database.Entity.Sql;
using Purple.Common.Database.Context.Sqlite;
using Purple.Common.ModelValidator;

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
    public ActionResult<List<ProductDTO>> Get()
    {
        try
        {
            List<ProductDTO> products = _purpleOcean.Products
                .Select(product => Mapping.Get<ProductDTO, Product>(product))
                .ToList();

            return products;
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
            var product = await _purpleOcean.Products
                .FirstOrDefaultAsync(product => product.Id == id);

            if (product is null)
                return NotFound($"Product with ID {id} not found");
            else
                return Ok(Mapping.Get<ProductDTO, Product>(product));
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }    
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] ProductDTO inputData)
    {
        try
        {
            Product product = Mapping.Get<Product, ProductDTO>(inputData);

            if (!Validate.TryValidate(inputData, out var results))
                this.ValidationProblems(results);
            else
                _purpleOcean.Add(product);

            await _purpleOcean.SaveChangesAsync();

            ProductDTO productDto = Mapping.Get<ProductDTO, Product>(product);
            return CreatedAtAction(
                nameof(Get),
                new { Id = product.Id },
                productDto
            );
        }
        catch (ArgumentNullException exception)
        {
            return BadRequest(exception.Message);
        }
    }

    [HttpPatch]
    public async Task<IActionResult> Patch([FromQuery] long id,
        [FromBody] ProductDTO inputData)
    {
        try
        {
            var product = _purpleOcean.Products
                .FirstOrDefault(product => product.Id == id);

            if (product is null)
                return NoContent();
            else
            {
                if (!Validate.TryValidate(inputData, out var results))
                    this.ValidationProblems(results);

                product.Name = inputData.Name is not null
                    ? inputData.Name
                    : product.Name;

                product.Description = inputData.Description is not null 
                    ? inputData.Description 
                    : product.Description;

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