using Purple.Common.Database.Entity.Sql;
using Purple.Common.Database.Context.Sqlite;

namespace Purple.Common.Database.Repository;

public class CustomerRepository : IRepository<Customer>
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
            .FirstOrDefault(customer => customer.FirstName == input.FirstName);

        if (customer is null)
            throw new ArgumentNullException("The returned DbContext object has a null value");

        return customer;
    }

    public async Task<Customer> Get(int customerId)
    {
        var customer = _context.Customers
            .FirstOrDefault(customer => customer.CustomerId == customerId);

        if (customer is null)
            throw new ArgumentNullException("The returned DbContext object has a null value");

        return customer;
    }
}