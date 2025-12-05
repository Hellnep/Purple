using PurpleBackendService.Domain.DTO;

namespace PurpleBackendService.Domain.Service
{
    public interface IProductService
    {
        public Task<OperationResult<ProductDTO>> CreateProductAsync(long id, ProductDTO input);
        public Task<OperationResult<ICollection<ProductDTO>>> GetProductsAsync();
        public Task<OperationResult<ICollection<ProductDTO>>> GetAuthorProductsAsync(long id);
        public Task<OperationResult<ProductDTO>> GetProductAsync(long id);
        public Task<OperationResult<ProductDTO>> ChangeProductAsync(long customerId, long productId, ProductDTO input);
    }
}

