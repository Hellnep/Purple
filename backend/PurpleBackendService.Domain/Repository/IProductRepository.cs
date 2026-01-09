using PurpleBackendService.Domain.Entity;

namespace PurpleBackendService.Domain.Repository
{
    public interface IProductRepository
    {
        public Task<Product> Add(Product input);

        public Task<Product?> Get(long productId);

        public Task<ICollection<Product>> Get();

        public Task<int> Update();

        public Task<Product?> Exists(long productId);
    }
}