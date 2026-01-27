using Microsoft.EntityFrameworkCore;

using PurpleBackendService.Domain.Entity;

namespace PurpleBackendService.Infrastucture.Sqlite
{
    public class PurpleOcean : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Product> Products { get; set; }

        public DbSet<Image> Images { get; set; }

        public PurpleOcean(DbContextOptions<PurpleOcean> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Linking the product model to the user model
            // Привязка модели продуктов к модели пользователей
            modelBuilder.Entity<User>()
                .HasMany(customer => customer.Products)
                .WithOne(product => product.Author)
                .HasForeignKey(product => product.AuthorRefId);

            // Linking the image model to the product model
            // Привязка модели изображений к модели продуктов
            modelBuilder.Entity<Product>()
                .HasMany(product => product.Images)
                .WithOne(image => image.Product)
                .HasForeignKey(image => image.ProductRefId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
