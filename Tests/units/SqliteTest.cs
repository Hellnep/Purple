using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Purple.Common.Database.DTO.Sql;
using Purple.Common.Database.Entity.Sql;
using Purple.Common.Database.Context.Sqlite;

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
                new Customer { Id = 1, Username = "Hellnep", Date = Today() }
            );
            context.SaveChanges();

            // Assert
            Assert.NotNull(context.Customers);
            Customer customer = context.Customers.First(customer => customer.Id == 1);

            Assert.Equal("Hellnep", customer.Username);
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
                new Product { Id = 1, Name = "Milk" },
                new Product { Id = 2, Name = "Orange", Description = "It's fucking orange" }
            );
            context.SaveChanges();

            // Assert
            Assert.NotNull(context.Products);
            Assert.Equal(2, context.Products.Count());
            Assert.Null(context.Products.Single(products => products.Id == 1).Description);
        }
    }

    [Fact]
    public void Mapping_CustomerDTO_inCustomer()
    {
        using (PurpleOcean context = new PurpleOcean(options))
        {
            // Act
            context.AddRange(
                new Customer { Id = 1, Username = "Hellnep", Date = Today() },
                new Customer { Id = 2, Username = "heats", Date = Today() }
            );
            context.SaveChanges();

            CustomerDTO customerDTO = context.Customers
                .Select(customer => new CustomerDTO { Username = customer.Username, Date = customer.Date, Id = customer.Id })
                .Single(customer => customer.Id == 1);

            // Assert
            Assert.NotNull(customerDTO);
            Assert.Equal("Hellnep", customerDTO.Username);
        }    
    }
}