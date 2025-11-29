using PurpleBackendService.Domain.Entity;

namespace PurpleBackendService.Core.Repository
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        public bool EmailExists(string email);
    }
}
