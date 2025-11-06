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
    public void Adding_a_buyer()
    {
        // Arrange
        using (PurpleOcean context = new PurpleOcean(options))
        {
            // Act
            context.Customers.Add(
                new Customer { FirstName = "Hellnep", Email = "hellnepya.ru" });
            context.SaveChanges();

            // Assert
            Assert.NotNull(context.Customers);
            Customer customer = context.Customers.First(customer => customer.Id == 1);

            Assert.Equal("Hellnep", customer.FirstName);
            Assert.Equal(Today(), customer.Date);
        }
    }

    [Fact]
    public void Adding_a_product()
    {
        // Arrange
        using (PurpleOcean context = new PurpleOcean(options))
        {
            // Act
            context.Products.AddRange(
                new Product { Name = "Milk" },
                new Product {
                    Name = "Orange",
                    Description = "It's fucking orange"
                }
            );
            context.SaveChanges();

            // Assert
            Assert.NotNull(context.Products);
            Assert.Equal(2, context.Products.Count());
            Assert.Null(context.Products.Single(products => products.Id == 1).Description);
        }
    }

    [Fact]
    public void Manual_Mapping_CustomerDTO_InCustomer()
    {
        using (PurpleOcean context = new PurpleOcean(options))
        {
            // Act
            context.AddRange(
                new Customer { FirstName = "Hellnep" },
                new Customer { FirstName = "heats" }
            );
            context.SaveChanges();

            CustomerDTO customerDTO = context.Customers
                .Select(customer => new CustomerDTO {
                    FirstName = customer.FirstName,
                    Date = customer.Date,
                    Id = customer.Id
                })
                .Single(customer => customer.Id == 1);

            // Assert
            Assert.NotNull(customerDTO);
            Assert.Equal("Hellnep", customerDTO.FirstName);
        }
    }

    [Fact]
    public void Auto_Mapping_From_CustomerDTO_InCustomer()
    {
        // Arrange
        using (PurpleOcean context = new PurpleOcean(options))
        {
            // Act
            CustomerDTO customer = new CustomerDTO
            {
                FirstName = "Hellnep",
                Email = "hellnep@ya.ru"
            };

            context.Add(Mapping.Get<Customer, CustomerDTO>(customer));
            context.SaveChanges();

            CustomerDTO newCustomer = Mapping.Get<CustomerDTO, Customer>(
                context.Customers.First(customer => customer.Id == 1));

            // Assert
            Assert.NotNull(newCustomer);
            Assert.Equal(1, context.Customers.Count());
            Assert.Equal(Today(),
                context.Customers.First(customer => customer.Id == 1).Date);
            Assert.Equal(customer.Email,
                context.Customers.First(customer => customer.Id == 1).Email);
        }
    }

    [Fact]
    public void Auto_Mapping_From_ProductDTO_InProduct()
    {
        // Arrange
        using (PurpleOcean context = new PurpleOcean(options))
        {
            // Act
            ProductDTO product = new ProductDTO
            {
                Name = "Coconut",
                Description = "Coconut is beautiful!"
            };

            context.Add(Mapping.Get<Product, ProductDTO>(product));
            context.SaveChanges();

            var newProduct = Mapping.Get<ProductDTO, Product>(
                context.Products.First(product => product.Id == 1));

            // Assert
            Assert.NotNull(newProduct);
            Assert.Equal(1, context.Products.Count());
        }
    }
}