using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Purple.Common.Database.Context.Sqlite;
using Purple.Common.Database.DTO.Sql;
using Purple.Common.Database.Repository;
using Purple.Common.Services;
using Purple.Common.Services.Interface;
using Purple.Tests.Utilities;

namespace Purple.Tests.Units;

public class CustomerServiceTest
{
    SqliteConnection connection;
    DbContextOptions<PurpleOcean> options;

    /// <summary>
    /// Adding database settings and connection settings
    /// during test initialization.
    /// </summary>
    public CustomerServiceTest()
    {
        connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();

        options = new DbContextOptionsBuilder<PurpleOcean>()
            .UseSqlite(connection)
            .Options;
    }

    [Theory]
    [MemberData(nameof(TestData.TestDataCustomerDTO), MemberType = typeof(TestData))]
    public async Task Add_ACustomer_Through(CustomerDTO input)
    {
        using (var context = new PurpleOcean(options))
        {
            // Arrange
            ICustomerRepository repository = new CustomerRepository(context);
            ICustomerService service = new CustomerService(repository);

            // Act
            var result = await service.CreateCustomerAsync(input);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotEmpty(context.Customers);

            Assert.Equal(input.FirstName, result.Data?.FirstName);
        }
    }

    [Theory]
    [MemberData(nameof(TestData.TestDataCustomerDTO), MemberType = typeof(TestData))]
    public async Task Get_ACustomer_Through(CustomerDTO input)
    {
        using (var context = new PurpleOcean(options))
        {
            // Arrange
            ICustomerRepository repository = new CustomerRepository(context);
            ICustomerService service = new CustomerService(repository);

            // Act
            await service.CreateCustomerAsync(input);
            var result = await service.GetCustomerAsync(1);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(input.Phone, result.Data?.Phone);
            Assert.Equal(input.Email, result.Data?.Email);
            Assert.Equal(input.FirstName, result.Data?.FirstName);
        }
    }

    [Fact]
    public async Task Get_List_OfCustomers()
    {
        using (var context = new PurpleOcean(options))
        {
            // Arrange
            ICustomerRepository repository = new CustomerRepository(context);
            ICustomerService service = new CustomerService(repository);

            var customer01 = new CustomerDTO()
            {
                FirstName = "Hellnep",
                Email = "example@test.com"
            };

            var customer02 = new CustomerDTO()
            {
                FirstName = "Никита",
                Email = "test@examlpe.com",
                Phone = "8(999)9999999"
            };

            await service.CreateCustomerAsync(customer01);
            await service.CreateCustomerAsync(customer02);

            // Act
            
            var result = await service.GetCustomersAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data?.Count());
        }
    }

    [Theory]
    [MemberData(nameof(TestData.TestDataCustomerDTO), MemberType = typeof(TestData))]
    public async Task Change_ACustomer_Through(CustomerDTO input)
    {
        using (var context = new PurpleOcean(options))
        {
            // Arrange
            ICustomerRepository repository = new CustomerRepository(context);
            ICustomerService service = new CustomerService(repository);

            var customer = new CustomerDTO()
            {
                FirstName = "Alien",
                Email = "example@alien.com",
                Phone = "0(111)1111111"
            };

            // Act
            await service.CreateCustomerAsync(input);
            var result = await service.ChangeCustomerAsync(1, customer);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotEqual(input.Phone, result.Data?.Phone);
            Assert.NotEqual(input.Email, result.Data?.Email);
            Assert.NotEqual(input.FirstName, result.Data?.FirstName);
        }
    }
}