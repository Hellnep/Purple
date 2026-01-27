using PurpleBackendService.Infrastucture.Utility;
using PurpleBackendService.Core.Interfaces.Services;
using PurpleBackendService.Core;
using PurpleBackendService.Core.DTOs.Product;
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
        /// <param name="input">Product metadata</param>
        /// <returns>
        /// Returns added product
        /// </returns>
        public async Task<OperationResult<ProductDTO>> CreateProductAsync(long id, ProductDTO input)
        {
            var product = Mapping
                .Get<Product, ProductDTO>(input);

            if (string.IsNullOrEmpty(product.Title))
            {
                return OperationResult<ProductDTO>.Failure("You need to enter a name for product");
            }

            product.AuthorRefId = id;
            var result = await _repository.Add(product);

            return OperationResult<ProductDTO>
                .Success(Mapping.Get<ProductDTO, Product>(result));
        }

        ///<summary>
        /// Get product from database
        /// </summary>
        /// <param name="productId">Product identificator</param>
        /// <returns>
        /// Returns product by identificator
        /// </returns>
        public async Task<OperationResult<ProductDTO>> GetProductAsync(long productId)
        {
            var result = await _repository.Get(productId);

            if (result is null)
            {
                return OperationResult<ProductDTO>
                    .Failure($"Product with ID={productId} not found");
            }

            return OperationResult<ProductDTO>
                .Success(Mapping.Get<ProductDTO, Product>(result));
        }

        ///<summary>
        /// Get all products from database
        /// </summary>
        /// <returns>
        /// Returns all products
        /// </returns>
        public async Task<OperationResult<ICollection<ProductDTO>>> GetProductsAsync()
        {
            var products = await _repository.Get();
            var result = new List<ProductDTO>();

            foreach (var product in products)
            {
                result.Add(Mapping.Get<ProductDTO, Product>(product));
            }

            return OperationResult<ICollection<ProductDTO>>
                .Success(result);
        }

        ///<summary>
        /// Get products of author from database
        /// </summary>
        /// <param name="authorRefId">User identificator</param>
        /// <returns>
        /// Returns products of author
        /// </returns>
        public async Task<OperationResult<ICollection<ProductDTO>>> GetAuthorProductsAsync(long authorRefId)
        {
            var products = await _repository.Get();
            var result = new List<ProductDTO>();

            foreach (var product in products)
            {
                if (product.AuthorRefId == authorRefId)
                {
                    result.Add(Mapping.Get<ProductDTO, Product>(product));
                }
            }

            return OperationResult<ICollection<ProductDTO>>
                .Success(result);
        }

        ///<summary>
        /// Change metadata of product in database
        /// </summary>
        /// <param name="productId">Product identificator</param>
        /// <param name="inputProduct">New metadata of product</param>
        /// <returns>
        /// Returns changes metadata of product
        /// </returns>
        public async Task<OperationResult<ProductDTO>> ChangeProductAsync(
            long productId,
            ProductDTO inputProduct
        )
        {
            var newData = Mapping.Get<Product, ProductDTO>(inputProduct);
            var product = await _repository.Get(productId);

            if (product is null)
            {
                return OperationResult<ProductDTO>
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

            return OperationResult<ProductDTO>
                .Success(Mapping.Get<ProductDTO, Product>(product));
        }
    }
}
