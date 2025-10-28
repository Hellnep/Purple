using Purple.Common.Database.Entity.Sqlite;

using Microsoft.EntityFrameworkCore;

namespace Purple.Common.Database.Context.Sqlite;

public class PurpleOcean : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }

    public PurpleOcean(DbContextOptions<PurpleOcean> options) : base(options)
    {
        Database.EnsureCreated();
    }
}