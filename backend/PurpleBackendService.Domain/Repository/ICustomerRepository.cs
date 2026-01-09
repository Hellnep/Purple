using PurpleBackendService.Domain.Entity;

namespace PurpleBackendService.Domain.Repository
{
    public interface ICustomerRepository
    {
        public Task<Customer> Add(Customer input);

        public Task<Customer?> Get(long id);

        public Task<ICollection<Customer>> Get();

        public Task<int> Update();

        public Task<bool> EmailExists(string email);
    }
}
