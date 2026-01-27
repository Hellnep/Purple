using PurpleBackendService.Domain.Entity;

namespace PurpleBackendService.Core.Interfaces.Services
{
    public interface IProductService
    {
        public Task<OperationResult<Product>> CreateProductAsync(long id, Product input);

        public Task<OperationResult<List<Product>>> GetProductsAsync();

        public Task<OperationResult<List<Product>>> GetAuthorProductsAsync(long id);

        public Task<OperationResult<Product>> GetProductAsync(long id);

        public Task<OperationResult<Product>> ChangeProductAsync(long productId, Product input);
    }
}

