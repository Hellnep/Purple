using Purple.Common.Database.Entity.Sql;

using Microsoft.EntityFrameworkCore;

namespace Purple.Common.Database.Context.Sqlite;

public class PurpleOcean : DbContext
{
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Product> Products { get; set; }

    public PurpleOcean(DbContextOptions<PurpleOcean> options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>()
            .HasMany(customer => customer.Products)
            .WithOne(product => product.Author)
            .HasForeignKey(product => product.AuthorRefId);

        base.OnModelCreating(modelBuilder);
    }
}