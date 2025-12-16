using PurpleBackendService.Domain.Entity;

namespace PurpleBackendService.Domain.Repository
{
    public interface ICustomerRepository
    {
        public Task<Customer> Add(Customer input);

        public Customer Get(long id);

        public ICollection<Customer> Get();

        public Task<int> Update();
        public bool EmailExists(string email);
    }
}
