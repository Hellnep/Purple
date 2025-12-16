using PurpleBackendService.Domain.Entity;

namespace PurpleBackendService.Domain.Repository
{
    public interface IProductRepository
    {
        public Task<Product> Add(Product input);

        public Product Get(long id);

        public ICollection<Product> Get();

        public Task<int> Update();
    }
}