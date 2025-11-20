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
            // Arrange        
            IRepository<Customer> repository = new CustomerRepository(context);

            // Act
            repository.Add(customer);
            var temp = repository.Get(1);

            // Assert
            Assert.Contains(customer, context.Customers);
            Assert.NotNull(temp.Products);
            
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
            // Arrange
            IRepository<Customer> customerRepository = new CustomerRepository(context);            
            IRepository<Product> productRepository = new ProductRepository(context);

            // Act
            customerRepository.Add(product.Author);
            productRepository.Add(product);
            
            var temp = productRepository.Get(1);

            // Assert
            Assert.Contains(product, context.Products);

            Assert.NotNull(temp.Author);
            
            Assert.Equal(product.Name, temp.Name);
            Assert.Equal(product.Description, temp.Description);
        }
    }

    [Theory]
    [MemberData(nameof(TestData.TestDataCustomer), MemberType = typeof(TestData))]
    public void Adding_And_Update_Customer_FromRepository(Customer input)
    {
        using(var context = new PurpleOcean(options))
        {
            // Arrange
            IRepository<Customer> repository = new CustomerRepository(context);
            repository.Add(input);

            // Act
            var customer = repository.Get(1);
            var template = new Customer
            {
                FirstName = "David",
                Email = "template@mail.com",
                Phone = "+6(888)7777777"
            };

            customer.FirstName = template.FirstName;
            customer.Email = template.Email;
            customer.Phone = template.Phone;

            repository.Update();

            // Assert
            Assert.Equal(template.Phone, customer.Phone);
            Assert.Equal(template.Email, customer.Email);
            Assert.Equal(template.FirstName, customer.FirstName);
        }
    }

    [Theory]
    [MemberData(nameof(TestData.TestDataProduct), MemberType = typeof(TestData))]
    public void Adding_And_Update_Product_FromRepository(Product input)
    {
        using(var context = new PurpleOcean(options))
        {
            // Arrange
            IRepository<Customer> customerRepository = new CustomerRepository(context);
            IRepository<Product> productRepository = new ProductRepository(context);
            
            customerRepository.Add(input.Author);
            productRepository.Add(input);

            // Act
            var product = productRepository.Get(1);
            var template = new Product
            {
                Name = "Chocolate",
                Description = "Is so tasty"
            };

            product.Name = template.Name;
            product.Description = template.Description;

            productRepository.Update();

            // Assert
            Assert.Equal(template.Name, product.Name);
            Assert.Equal(template.Description, product.Description);
        }
    }

    [Theory]
    [MemberData(nameof(TestData.TestDataCustomer), MemberType = typeof(TestData))]
    public void Testing_Method_Checking_Email(Customer input)
    {
        using (var context = new PurpleOcean(options))
        {
            // Arrange
            ICustomerRepository repository = new CustomerRepository(context);
            repository.Add(input);

            // Act
            string email = "example@test.com";
            bool isValid = repository.EmailExists(email);

            // Assert
            if (isValid)
                Assert.True(isValid);
            else
                Assert.False(isValid);
        }
    }

    [Theory]
    [MemberData(nameof(TestData.TestDataCustomer), MemberType = typeof(TestData))]
    public void Catch_Exception(Customer input)
    {
        using (var context = new PurpleOcean(options))
        {
            // Arrange
            ICustomerRepository customerRepository = new CustomerRepository(context);
            IRepository<Product> productRepository = new ProductRepository(context);
            
            customerRepository.Add(input);

            // Act
            var exCustomer = Assert.Throws<ArgumentNullException>(() => 
                customerRepository.Get(context.Customers.Count() + 1));

            var exProduct = Assert.Throws<ArgumentNullException>(() => 
                productRepository.Get(context.Products.Count() + 1));

            // Assert
            Assert.Equal("The returned DbContext object has a null value", exProduct.ParamName);
            Assert.Equal("The returned DbContext object has a null value", exCustomer.ParamName);
        }
    }
}