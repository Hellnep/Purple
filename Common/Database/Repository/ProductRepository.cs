using Purple.Common.Database.Entity.Sql;
using Purple.Common.Database.Context.Sqlite;

namespace Purple.Common.Database.Repository;

public class ProductRepository : IRepository<Product>
{
    private PurpleOcean _context;

    public ProductRepository(PurpleOcean context)
    {
        _context = context;
    }

    public async Task<Product> Add(Product input)
    {
        _context.Products.Add(input);
        await _context.SaveChangesAsync();
        
        var product = _context.Products
            .FirstOrDefault(product => product.Name == input.Name);

        if (product is null)
            throw new ArgumentNullException("The returned DbContext object has a null value");

        return product;
    }

    public async Task<Product> Get(int productId)
    {
        var product = _context.Products
            .FirstOrDefault(product => product.ProductId == productId);

        if (product is null)
            throw new ArgumentNullException("The returned DbContext object has a null value");

        return product;
    }
}