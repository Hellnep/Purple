using Microsoft.EntityFrameworkCore;

using PurpleBackendService.Domain.Entity;
using PurpleBackendService.Domain.Repository;
using PurpleBackendService.Infrastructure.Sqlite;

namespace PurpleBackendService.Core.Repository
{
    public class ProductRepository : Repository, IProductRepository
    {
        public ProductRepository(PurpleOcean repository) : base(repository)
        {
        }

        public async Task<Product> Add(Product product)
        {
            if (product.AuthorRefId is null)
            {
                throw new ArgumentNullException("Author for a new product should not be null");
            }

            long authorRefId = (long)product.AuthorRefId;
            var customer = await _repository.Customers
                .Include(customer => customer.Products)
                .FirstOrDefaultAsync(customer => customer.Id == authorRefId);

            if (customer is null)
            {
                throw new ArgumentNullException($"Customer with Id={authorRefId} not found");
            }

            customer.Products ??= new List<Product>();
            customer.Products.Add(product);

            await _repository.SaveChangesAsync();
            return product;
        }

        public Product Get(long id)
        {
            var product = _repository.Products
                .Include(product => product.Author)
                .Include(product => product.Images)
                .FirstOrDefault(product => product.Id == id);

            if (product is null)
            {
                throw new ArgumentNullException("The returned DbContext object has a null value");
            }

            return product;
        }

        public ICollection<Product> Get()
        {
            var products = _repository.Products
                .Include(product => product.Author)
                .Include(product => product.Images)
                .ToList();

            return products;
        }

        public async Task<int> Update() =>
            await _repository.SaveChangesAsync();
    }
}
