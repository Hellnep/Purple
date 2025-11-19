using Microsoft.EntityFrameworkCore;

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
        if (input.Author is null)
            throw new ArgumentNullException("Author for a new product should not be null");

        long authorRefId = input.Author.CustomerId;
        var customer = _context.Customers
            .Include(customer => customer.Products)
            .First(customer => customer.CustomerId == authorRefId);

        if (customer is null)
            throw new ArgumentNullException("Customer does not exist");

        customer.Products?.Add(input);
        await _context.SaveChangesAsync();
        
        var product = _context.Products
            .Where(product => product.AuthorRefId == authorRefId)
            .First(product => product.Name == input.Name);

        return product;
    }

    public Product Get(long id)
    {
        var product = _context.Products
            .FirstOrDefault(product => product.ProductId == id);

        if (product is null)
            throw new ArgumentNullException("The returned DbContext object has a null value");

        return product;
    }
}