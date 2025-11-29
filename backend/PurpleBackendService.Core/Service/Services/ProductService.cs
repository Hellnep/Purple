using PurpleBackendService.Core;
using PurpleBackendService.Core.Repository;
using PurpleBackendService.Domain.DTO;
using PurpleBackendService.Domain.Entity;
using PurpleBackendService.Domain.Utility;

namespace PurpleBackendService.Domain.Service;

public class ProductService : IProductService
{
    private IRepository<Product> _repository;

    public ProductService(IRepository<Product> repository)
    {
        _repository = repository;
    }

    public async Task<OperationResult<ProductDTO>> CreateProductAsync(long id, ProductDTO input)
    {
        Product product = Mapping
            .Get<Product, ProductDTO>(input);

        if (string.IsNullOrEmpty(product.Name))
            return OperationResult<ProductDTO>.Failure("You need to enter a name for product");

        product.AuthorRefId = id;
        var result = await _repository.Add(product);

        return OperationResult<ProductDTO>
            .Success(Mapping.Get<ProductDTO, Product>(result));
    }

    public async Task<OperationResult<ProductDTO>> GetProductAsync(long id)
    {
        var result = _repository.Get(id);

        return OperationResult<ProductDTO>
            .Success(Mapping.Get<ProductDTO, Product>(result));
    }

    public async Task<OperationResult<ICollection<ProductDTO>>> GetProductsAsync()
    {
        var products = _repository.Get();
        var result = new List<ProductDTO>();

        foreach (Product product in products)
            result.Add(Mapping.Get<ProductDTO, Product>(product));

        return OperationResult<ICollection<ProductDTO>>
            .Success(result);
    }

    public async Task<OperationResult<ICollection<ProductDTO>>> GetAuthorProductsAsync(long id)
    {
        var products = _repository.Get();
        var result = new List<ProductDTO>();

        foreach (Product product in products)
            if (product.AuthorRefId == id)
                result.Add(Mapping.Get<ProductDTO, Product>(product));

        return OperationResult<ICollection<ProductDTO>>
            .Success(result);
    }

    public async Task<OperationResult<ProductDTO>> ChangeProductAsync(long customerId, long productId, ProductDTO input)
    {
        try
        {
            var newData = Mapping.Get<Product, ProductDTO>(input);
            var product = _repository.Get()
                .Where(product => product.AuthorRefId == customerId)
                .FirstOrDefault(product => product.ProductId == productId);

            if (product is null)
                return OperationResult<ProductDTO>.Failure($"Product with Id={productId} do not exists");

            if (!string.IsNullOrEmpty(newData.Name))
                product.Name = newData.Name;

            if (!string.IsNullOrEmpty(newData.Description))
                product.Description = newData.Description;

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