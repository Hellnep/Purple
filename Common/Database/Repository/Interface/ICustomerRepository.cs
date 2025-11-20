using Purple.Common.Database.Entity.Sql;

namespace Purple.Common.Database.Repository;

public interface ICustomerRepository : IRepository<Customer>
{
    public bool EmailExists(string email);
}