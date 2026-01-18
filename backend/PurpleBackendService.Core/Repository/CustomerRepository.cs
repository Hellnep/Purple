using Microsoft.EntityFrameworkCore;

using PurpleBackendService.Domain.Entity;
using PurpleBackendService.Domain.Repository;
using PurpleBackendService.Infrastructure.Sqlite;

namespace PurpleBackendService.Core.Repository
{
    public class CustomerRepository : Repository, ICustomerRepository
    {
        public CustomerRepository(PurpleOcean repository) : base(repository)
        {
        }

        public Task<User> Add(User customer)
        {
            _repository.Customers.Add(customer);
            return _repository
                .SaveChangesAsync()
                .ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        return customer;
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to save changes");
                    }
                });
        }

        public Task<User?> Get(long customerId) =>
            _repository.Customers
                .Include(customer => customer.Products)
                .FirstOrDefaultAsync(customer => customer.Id == customerId);


        public Task<bool> EmailExists(string email) =>
            _repository.Customers
                .AnyAsync(customer => string.Equals(customer.Email, email));

        public Task<int> Update() =>
            _repository.SaveChangesAsync();

        public Task<ICollection<User>> Get() =>
            Task.FromResult(_repository.Customers
                .Include(customer => customer.Products)
                .ToList() as ICollection<User>
            );
    }
}