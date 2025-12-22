using PurpleBackendService.Domain.DTO;

namespace PurpleBackendService.Domain.Service
{
    public interface IProductService
    {
        public Task<OperationResult<ProductDTO>> CreateProductAsync(long id, ProductDTO input);

        public OperationResult<ICollection<ProductDTO>> GetProducts();

        public OperationResult<ICollection<ProductDTO>> GetAuthorProducts(long id);

        public OperationResult<ProductDTO> GetProduct(long id);

        public Task<OperationResult<ProductDTO>> ChangeProductAsync(long customerId, long productId, ProductDTO input);
    }
}

