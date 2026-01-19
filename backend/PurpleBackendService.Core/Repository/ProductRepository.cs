using Microsoft.EntityFrameworkCore;

using PurpleBackendService.Domain.Entity;
using PurpleBackendService.Domain.Repository;
using PurpleBackendService.Infrastructure.Sqlite;

namespace PurpleBackendService.Core.Repository
{
    public class ProductRepository(PurpleOcean repository)
        : Repository(repository), IProductRepository
    {
        ///<summary>
        /// Add product data to database
        /// </summary>
        /// <param name="product">Product data</param>
        /// <returns>Product entity</returns>
        public Task<Product> Add(Product product)
        {
            long authorRefId = (long)product.AuthorRefId!;
            var customer = _repository.Users
                .Include(customer => customer.Products)
                .FirstOrDefault(customer => customer.Id == authorRefId) ??
                    throw new ArgumentNullException($"Customer with Id={authorRefId} not found");

            customer.Products ??= [];
            customer.Products.Add(product);

            // This is where the changes are saved the result is returned
            // Здесь происходит сохранение изменений, с последующим возвратом результата
            return _repository
                .SaveChangesAsync()
                .ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        return product;
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to save changes");
                    }
                });
        }

        ///<summary>
        /// Check if product exists
        /// </summary>
        /// <param name="productId">Product identificator</param>
        /// <returns>Product entity</returns>
        public  Task<Product?> Exists(long productId) =>
            Task.FromResult(_repository.Products.Find(productId));

        ///<summary>
        /// Get product by id
        /// </summary>
        /// <param name="id">Product identificator</param>
        /// <returns>Product entity</returns>
        public Task<Product?> Get(long id) =>
            _repository.Products
                .Include(product => product.Author)
                .Include(product => product.Images)
                .FirstOrDefaultAsync(product => product.Id == id);

        ///<summary>
        /// Get all products from database
        /// </summary>
        /// <returns>List of products</returns>
        public Task<ICollection<Product>> Get() =>
            Task.FromResult(_repository.Products
                .Include(product => product.Author)
                .Include(product => product.Images)
                .ToList() as ICollection<Product>
            );

        ///<summary>
        /// Update datas in database
        /// </summary>
        public Task<int> Update() =>
            _repository.SaveChangesAsync();
    }
}
