using PurpleBackendService.Core.DTOs.Product;

namespace PurpleBackendService.Core.Interfaces.Services
{
    public interface IProductService
    {
        public Task<OperationResult<ProductDTO>> CreateProductAsync(long id, ProductDTO input);

        public Task<OperationResult<ICollection<ProductDTO>>> GetProductsAsync();

        public Task<OperationResult<ICollection<ProductDTO>>> GetAuthorProductsAsync(long id);

        public Task<OperationResult<ProductDTO>> GetProductAsync(long id);

        public Task<OperationResult<ProductDTO>> ChangeProductAsync(long productId, ProductDTO input);
    }
}

