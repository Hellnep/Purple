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
        if (input.AuthorRefId is null)
            throw new ArgumentNullException("Author for a new product should not be null");

        long authorRefId = (long)input.AuthorRefId;
        var customer = await _context.Customers
            .Include(customer => customer.Products)
            .FirstOrDefaultAsync(customer => customer.CustomerId == authorRefId);

        if (customer is null)
            throw new ArgumentNullException($"Customer with Id={authorRefId} not found");

        customer.Products ??= new List<Product>();
        customer.Products.Add(input);

        await _context.SaveChangesAsync();

        return input;
    }

    public Product Get(long id)
    {
        var product = _context.Products
            .Include(product => product.Author)
            .FirstOrDefault(product => product.ProductId == id);

        if (product is null)
            throw new ArgumentNullException("The returned DbContext object has a null value");

        return product;
    }

    public ICollection<Product> Get()
    {
        var products = _context.Products
            .Include(product => product.Author)
            .ToList();

        return products;
    }

    public async Task<int> Update() =>
        await _context.SaveChangesAsync();
}