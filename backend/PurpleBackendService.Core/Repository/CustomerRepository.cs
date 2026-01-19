using Microsoft.EntityFrameworkCore;

using PurpleBackendService.Domain.Entity;
using PurpleBackendService.Domain.Repository;
using PurpleBackendService.Infrastructure.Sqlite;

namespace PurpleBackendService.Core.Repository
{
    public class CustomerRepository(PurpleOcean repository)
        : Repository(repository), ICustomerRepository
    {
        ///<summary>
        ///Add customer to database
        ///</summary>
        ///<param name="customer">Customer to add</param>
        ///<returns>
        ///The result of adding a customer
        ///</returns>
        public Task<User> Add(User customer)
        {
            _repository.Users.Add(customer);
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
            _repository.Users
                .Include(customer => customer.Products)
                .FirstOrDefaultAsync(customer => customer.Id == customerId);

        ///<summary>
        ///Check if email exists in database
        ///</summary>
        ///<param name="email">Email to check</param>
        ///<returns>
        ///The result of checking the existence an email
        ///</returns>
        public Task<bool> EmailExists(string email) =>
            _repository.Users
                .AnyAsync(customer => string.Equals(customer.Email, email));

        ///<summary>
        ///Update datas in database
        ///</summary>
        public Task<int> Update() =>
            _repository.SaveChangesAsync();

        ///<summary>
        ///Get all customers from database
        ///</summary>
        ///<returns>List of customers</returns>
        public Task<ICollection<User>> Get() =>
            Task.FromResult(_repository.Users
                .Include(customer => customer.Products)
                .ToList() as ICollection<User>
            );
    }
}