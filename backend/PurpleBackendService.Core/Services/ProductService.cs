using PurpleBackendService.Core;
using PurpleBackendService.Core.Utility;
using PurpleBackendService.Core.Interfaces.Services;
using PurpleBackendService.Domain.Entity;
using PurpleBackendService.Domain.Interfaces.Repositories;

namespace PurpleBackendService.Infrastucture.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        ///<summary>
        /// Adds a product to the database
        /// </summary>
        /// <param name="id">User identificator</param>
        /// <param name="product">Product metadata</param>
        /// <returns>
        /// Returns added product
        /// </returns>
        public async Task<OperationResult<Product>> CreateProductAsync(long id, Product product)
        {
            if (string.IsNullOrEmpty(product.Title))
            {
                return OperationResult<Product>.Failure("You need to enter a name for product");
            }

            product.AuthorRefId = id;
            var result = await _repository.Add(product);

            return OperationResult<Product>
                .Success(result);
        }

        ///<summary>
        /// Get product from database
        /// </summary>
        /// <param name="productId">Product identificator</param>
        /// <returns>
        /// Returns product by identificator
        /// </returns>
        public async Task<OperationResult<Product>> GetProductAsync(long productId)
        {
            var result = await _repository.Get(productId);

            if (result is null)
            {
                return OperationResult<Product>
                    .Failure($"Product with ID={productId} not found");
            }

            return OperationResult<Product>
                .Success(result);
        }

        ///<summary>
        /// Get all products from database
        /// </summary>
        /// <returns>
        /// Returns all products
        /// </returns>
        public async Task<OperationResult<List<Product>>> GetProductsAsync()
        {
            var products = await _repository.Get() as List<Product>;

            return OperationResult<List<Product>>
                .Success(products!);
        }

        ///<summary>
        /// Get products of author from database
        /// </summary>
        /// <param name="authorRefId">User identificator</param>
        /// <returns>
        /// Returns products of author
        /// </returns>
        public async Task<OperationResult<List<Product>>> GetAuthorProductsAsync(long authorRefId)
        {
            var products = await _repository.Get();
            var result = new List<Product>();

            foreach (var product in products)
            {
                if (product.AuthorRefId == authorRefId)
                {
                    result.Add(product);
                }
            }

            return OperationResult<List<Product>>
                .Success(result);
        }

        ///<summary>
        /// Change metadata of product in database
        /// </summary>
        /// <param name="productId">Product identificator</param>
        /// <param name="newData">New metadata of product</param>
        /// <returns>
        /// Returns changes metadata of product
        /// </returns>
        public async Task<OperationResult<Product>> ChangeProductAsync(
            long productId,
            Product newData
        )
        {
            var product = await _repository.Get(productId);

            if (product is null)
            {
                return OperationResult<Product>
                    .Failure($"Product with Id={productId} do not exists");
            }

            if (!string.IsNullOrEmpty(newData.Title))
            {
                product.Title = newData.Title;
            }

            if (!string.IsNullOrEmpty(newData.Content))
            {
                product.Content = newData.Content;
            }

            await _repository.Update();

            return OperationResult<Product>
                .Success(product);
        }
    }
}
