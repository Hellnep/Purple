using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Purple.Common.Database.DTO.Sql;
using Purple.Common.Database.Entity.Sql;
using Purple.Common.Database.Context.Sqlite;
using Purple.Common.Database.Mapping;

namespace Purple.Tests.Units;

public class SqliteTest
{
    SqliteConnection connection;
    DbContextOptions<PurpleOcean> options;

    /// <summary>
    /// Adding database settings and connection settings
    /// during test initialization.
    /// </summary>
    public SqliteTest()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        options = new DbContextOptionsBuilder<PurpleOcean>()
            .UseSqlite(connection)
            .Options;
    }

    /// <summary>
    /// The return of today is date.
    /// </summary>
    /// <returns>Returns a DateOnly object.</returns>
    private static DateOnly Today() => DateOnly.FromDateTime(DateTime.Now);

    [Fact]
    public void Adding_a_Customer()
    {
        
        using (PurpleOcean context = new PurpleOcean(options))
        {
            // Arrange
            context.Database.EnsureCreated();

            var dtoCustomer = new CustomerDTO()
            {
                FirstName = "Hellnep",
                Email = "hellnep@ya.ru",
                Phone = "8(999)9999999"
            };
            
            // Act
            var customer = Mapping.Get<Customer, CustomerDTO>(dtoCustomer);

            context.Add(customer);
            context.SaveChanges();

            Assert.NotNull(context.Customers);

            // Assert

            var checkCustomer = context.Customers.First(customer => customer.CustomerId == 1);
            var mappedCustomer = Mapping.Get<CustomerDTO, Customer>(checkCustomer);

            Assert.Equal(customer.FirstName, mappedCustomer.FirstName);
            Assert.Equal(dtoCustomer.Email, mappedCustomer.Email);
            Assert.Equal(checkCustomer.Phone, mappedCustomer.Phone);
            Assert.Equal(Today(), checkCustomer.Date);

            context.Database.EnsureDeleted();
        }
    }

    [Fact]
    public void Adding_a_Product()
    {
        using (PurpleOcean context = new PurpleOcean(options))
        {
            // Arrange
            context.Database.EnsureCreated();

            var customerDTO = new CustomerDTO
            {
                FirstName = "Денис",
                Email = "hellnep@ya.ru"
            };

            context.Customers.Add(Mapping.Get<Customer, CustomerDTO>(customerDTO));
            context.SaveChanges();

            Assert.NotEmpty(context.Customers);
            Assert.Equal(1, context.Customers
                .First(customer => customer.FirstName == "Денис").CustomerId);
            
            // Act
            var productDTO = new ProductDTO
            {
                Name = "Решение задач",
                Description = "Пример решения задач."
            };

            var product = Mapping.Get<Product, ProductDTO>(productDTO);

            var customer = context.Customers
                .Include(customer => customer.Products)
                .First(customer => customer.CustomerId == 1);
            
            Assert.NotNull(customer.Products);

            customer.Products.Add(product);
            context.SaveChanges();

            // Assert
            var dtoProduct = Mapping.Get<ProductDTO, Product>(
                context.Products.First(product => product.ProductId == 1));

            var dtoCustomer = Mapping.Get<CustomerDTO, Customer>(
                context.Customers.First(customer => customer.CustomerId == 1));
            
            Assert.NotNull(dtoProduct.Author);
            Assert.Equal(Today(), dtoProduct.Author.Date);
            Assert.Equal(dtoCustomer.Email, dtoProduct.Author.Email);
            Assert.Equal(dtoCustomer.FirstName, dtoProduct.Author.FirstName);

            context.Database.EnsureDeleted();
        }
    }
}