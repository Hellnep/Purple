using PurpleBackendService.Domain.Entity;

namespace PurpleBackendService.Domain.Repository
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        public bool EmailExists(string email);
    }
}
