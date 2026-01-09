using System.Threading.Tasks;
using PurpleBackendService.Core.Utility;
using PurpleBackendService.Domain.DTO;
using PurpleBackendService.Domain.Entity;
using PurpleBackendService.Domain.Repository;
using PurpleBackendService.Domain.Service;


namespace PurpleBackendService.Core.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _repository;

        public ProductService(IProductRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult<ProductDTO>> CreateProductAsync(long id, ProductDTO input)
        {
            Product product = Mapping
                .Get<Product, ProductDTO>(input);

            if (string.IsNullOrEmpty(product.Title))
                return OperationResult<ProductDTO>.Failure("You need to enter a name for product");

            product.AuthorRefId = id;
            var result = await _repository.Add(product);

            return OperationResult<ProductDTO>
                .Success(Mapping.Get<ProductDTO, Product>(result));
        }

        public async Task<OperationResult<ProductDTO>> GetProduct(long id)
        {
            var result = await _repository.Get(id);

            return OperationResult<ProductDTO>
                .Success(Mapping.Get<ProductDTO, Product>(result));
        }

        public async Task<OperationResult<ICollection<ProductDTO>>> GetProducts()
        {
            var products = await _repository.Get();
            var result = new List<ProductDTO>();

            foreach (Product product in products)
                result.Add(Mapping.Get<ProductDTO, Product>(product));

            return OperationResult<ICollection<ProductDTO>>
                .Success(result);
        }

        public async Task<OperationResult<ICollection<ProductDTO>>> GetAuthorProducts(long authorRefId)
        {
            var products = await _repository.Get();
            var result = new List<ProductDTO>();

            foreach (Product product in products)
            {
                if (product.AuthorRefId == authorRefId)
                    result.Add(Mapping.Get<ProductDTO, Product>(product));
            }

            return OperationResult<ICollection<ProductDTO>>
                .Success(result);
        }

        public async Task<OperationResult<ProductDTO>> ChangeProductAsync(long customerId, long productId, ProductDTO inputProduct)
        {
            try
            {
                var newData = Mapping.Get<Product, ProductDTO>(inputProduct);
                var products = await _repository.Get();

                var product = products
                    .Where(product => product.AuthorRefId == customerId)
                    .FirstOrDefault(product => product.Id == productId);

                if (product is null)
                    return OperationResult<ProductDTO>.Failure($"Product with Id={productId} do not exists");

                if (!string.IsNullOrEmpty(newData.Title))
                    product.Title = newData.Title;

                if (!string.IsNullOrEmpty(newData.Content))
                    product.Content = newData.Content;

                await _repository.Update();

                return OperationResult<ProductDTO>
                    .Success(Mapping.Get<ProductDTO, Product>(product));
            }
            catch (ArgumentNullException exception)
            {
                return OperationResult<ProductDTO>.Failure(exception.Message);
            }
        }
    }
}