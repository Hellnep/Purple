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

        public async Task<Customer> Add(Customer input)
        {
            _repository.Customers.Add(input);
            await _repository.SaveChangesAsync();
            
            var customer = _repository.Customers
                .First(customer => customer.Nickname == input.Nickname);

            return customer;
        }

        public Customer Get(long id)
        {
            var customer = _repository.Customers
                .Include(customer => customer.Products)
                .FirstOrDefault(customer => customer.Id == id);

            if (customer is null)
                throw new ArgumentNullException("The returned DbContext object has a null value");

            return customer;
        }

        public bool EmailExists(string email) =>
            _repository.Customers
                .Any(customer => string.Equals(customer.Email, email));

        public async Task<int> Update() =>
            await _repository.SaveChangesAsync();

        public ICollection<Customer> Get()
        {
            var customers = _repository.Customers
                .Include(customer => customer.Products)
                .ToList();

            return customers;
        }
    }
}