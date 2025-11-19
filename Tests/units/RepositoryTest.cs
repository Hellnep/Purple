using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Purple.Tests.Utilities;

using Purple.Common.Database.Context.Sqlite;
using Purple.Common.Database.Entity.Sql;
using Purple.Common.Database.Repository;

namespace Purple.Tests.Units;

public class RepositoryTest
{
    SqliteConnection connection;
    DbContextOptions<PurpleOcean> options;

    /// <summary>
    /// Adding database settings and connection settings
    /// during test initialization.
    /// </summary>
    public RepositoryTest()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        options = new DbContextOptionsBuilder<PurpleOcean>()
            .UseSqlite(connection)
            .Options;
    }

    [Theory]
    [MemberData(nameof(TestData.TestDataCustomer), MemberType = typeof(TestData))]
    public void Adding_And_Get_Customer_FromRepository(Customer customer)
    {
        using(var context = new PurpleOcean(options))
        {
            // Act            
            IRepository<Customer> repository = new CustomerRepository(context);

            repository.Add(customer);
            var temp = context.Customers.First();

            // Assert
            Assert.Contains(customer, context.Customers);
            
            Assert.Equal(customer.Email, temp.Email);
            Assert.Equal(customer.FirstName, temp.FirstName);
        }
    }

    [Theory]
    [MemberData(nameof(TestData.TestDataProduct), MemberType = typeof(TestData))]
    public void Adding_And_Get_Product_FromRepository(Product product)
    {
        using(var context = new PurpleOcean(options))
        {
            // Act            
            IRepository<Product> repository = new ProductRepository(context);

            repository.Add(product);
            var temp = context.Products.First();

            // Assert
            Assert.Contains(product, context.Products);

            Assert.NotNull(temp.Author);
            
            Assert.Equal(product.Name, temp.Name);
            Assert.Equal(product.Description, temp.Description);
        }
    }
}