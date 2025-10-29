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

    public SqliteTest()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        options = new DbContextOptionsBuilder<PurpleOcean>()
            .UseSqlite(connection)
            .Options;
    }

    private DateOnly Today() => DateOnly.FromDateTime(DateTime.Now);

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
            Assert.NotNull(context.Customers.Single(customer => customer.Id == 1));
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
                .Select(customer => new CustomerDTO { Username = customer.Username, Date = customer.Date })
                .Single(customer => customer.Username == "Hellnep");

            // Assert
            Assert.NotNull(customerDTO);
        }    
    }
}