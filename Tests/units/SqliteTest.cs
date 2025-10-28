using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Purple.Common.Database.Entity.Sql;
using Purple.Common.Database.Context.Sqlite;

namespace Purple.Tests.Unit;

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

    [Fact]
    public void Adding_a_buyer()
    {
        // Arrange
        using (PurpleOcean context = new PurpleOcean(options))
        {
            // Act
            context.Customers.Add(
                new Customer { Id = 1, Username = "Hellnep", Date = DateOnly.FromDateTime(DateTime.Now) }
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
}