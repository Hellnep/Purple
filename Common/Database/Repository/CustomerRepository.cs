using Purple.Common.Database.Entity.Sql;
using Purple.Common.Database.Context.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Purple.Common.Database.Repository;

public class CustomerRepository : ICustomerRepository
{
    private PurpleOcean _context;

    public CustomerRepository(PurpleOcean context)
    {
        _context = context;
    }

    public async Task<Customer> Add(Customer input)
    {
        _context.Customers.Add(input);
        await _context.SaveChangesAsync();
        
        var customer = _context.Customers
            .First(customer => customer.FirstName == input.FirstName);

        return customer;
    }

    public Customer Get(long id)
    {
        var customer = _context.Customers
            .Include(customer => customer.Products)
            .FirstOrDefault(customer => customer.CustomerId == id);

        if (customer is null)
            throw new ArgumentNullException("The returned DbContext object has a null value");

        return customer;
    }

    public bool EmailExists(string email) =>
        _context.Customers
            .Any(customer => string.Equals(customer.Email, email));

    public async Task<int> Update() =>
        await _context.SaveChangesAsync();
}