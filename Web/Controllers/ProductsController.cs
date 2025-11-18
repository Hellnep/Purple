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
                .Include(product => product.Author)
                .Select(product => Mapping.Get<ProductDTO, Product>(product))
                .ToList();

            return Ok(products);
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDTO>> Get(long id)
    {
        try
        {
            ProductDTO? product = Mapping.Get<ProductDTO, Product>(
                _purpleOcean.Products
                    .Include(product => product.Author)
                    .First(product => product.ProductId == id)
                );

            if (product is null)
                return NotFound($"Product with ID {id} not found");
            else
                return Ok(product);
        }
        catch (ArgumentNullException exception)
        {
            return NotFound(exception.Message);
        }    
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromQuery] long id, [FromBody] ProductDTO inputData)
    {
        try
        {
            if (!Validate.TryValidate(inputData, out var results))
                this.ValidationProblems(results);
            else
            {
                var newProduct = Mapping.Get<Product, ProductDTO>(inputData);
                var customer = _purpleOcean.Customers
                    .Include(customer => customer.Products)
                    .First(customer => customer.CustomerId == id);

                customer.Products.Add(newProduct);
                await _purpleOcean.SaveChangesAsync();
            }

            var product = _purpleOcean.Products
                .First(product => product.Name == inputData.Name);

            ProductDTO productDto = Mapping.Get<ProductDTO, Product>(product);
            return CreatedAtAction(
                nameof(Get),
                new { Id = productDto.ProductId },
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
                .FirstOrDefault(product => product.ProductId == id);

            if (product is null)
                return NotFound();
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