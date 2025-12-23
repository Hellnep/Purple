using Microsoft.EntityFrameworkCore;

using PurpleBackendService.Domain.Entity;

namespace PurpleBackendService.Infrastructure.Sqlite
{
    public class PurpleOcean : DbContext
    {
        public DbSet<Customer> Customers { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Image> Images { get; set; }

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

            modelBuilder.Entity<Product>()
                .HasMany(product => product.Images)
                .WithOne(image => image.Product)
                .HasForeignKey(image => image.ProductRefId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
