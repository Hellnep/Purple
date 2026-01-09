using PurpleBackendService.Domain.DTO;

namespace PurpleBackendService.Domain.Service
{
    public interface IProductService
    {
        public Task<OperationResult<ProductDTO>> CreateProductAsync(long id, ProductDTO input);

        public Task<OperationResult<ICollection<ProductDTO>>> GetProducts();

        public Task<OperationResult<ICollection<ProductDTO>>> GetAuthorProducts(long id);

        public Task<OperationResult<ProductDTO>> GetProduct(long id);

        public Task<OperationResult<ProductDTO>> ChangeProductAsync(long customerId, long productId, ProductDTO input);
    }
}

