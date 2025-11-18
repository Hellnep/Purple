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
        // Arrange
        using (PurpleOcean context = new PurpleOcean(options))
        {
            // Act
            Customer customer = new()
            {
                FirstName = "Hellnep",
                Email = "hellnep@ya.ru"
            };

            context.Add(customer);
            context.SaveChanges();

            // Assert
            Assert.NotNull(context.Customers);
            Customer newCustomer = context.Customers.First(customer => customer.CustomerId == 1);

            Assert.Equal("Hellnep", customer.FirstName);
            Assert.Equal(Today(), customer.Date);
        }
    }

    [Fact]
    public void Adding_a_Product_FromCustomer()
    {
        using (PurpleOcean context = new PurpleOcean(options))
        {
            Customer customer = new Customer
            {
                FirstName = "Денис",
                Email = "hellnep@ya.ru"
            };

            context.Customers.Add(customer);
            context.SaveChanges();

            Assert.NotNull(context.Customers);

            Product product = new Product
            {
                Name = "Титульник",
                Description = "Описание"
            };

            var products = context.Customers
                .Include(customer => customer.Products)
                .First(customer => customer.CustomerId == 1);

            products.Products.Add(product);
            context.SaveChanges();

            Assert.NotNull(context.Products);
            Assert.Equal(
                context.Customers.First(customer => customer.FirstName == "Денис").CustomerId,
                context.Products.First(product => product.Name == "Титульник").AuthorRefId
            );
        }
    }

    [Fact]
    public void Adding_a_Products()
    {
        // Arrange
        using (PurpleOcean context = new PurpleOcean(options))
        {
            // Act
            context.Customers.Add(new Customer
            {
                FirstName = "Денис"
            });
            context.SaveChanges();

            context.Products.AddRange(
                new Product { Name = "Milk", AuthorRefId = 1 },
                new Product {
                    Name = "Orange",
                    Description = "It's fucking orange",
                    AuthorRefId = 1
                }
            );
            context.SaveChanges();

            // Assert
            Assert.NotNull(context.Products);
            Assert.Equal(2, context.Products.Count());
            Assert.Null(context.Products.Single(products => products.ProductId == 1).Description);
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

            Customer customerDTO = context.Customers
                .Select(customer => new Customer {
                    FirstName = customer.FirstName,
                    Date = customer.Date,
                    CustomerId = customer.CustomerId
                })
                .Single(customer => customer.CustomerId == 1);

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
                context.Customers.First(customer => customer.CustomerId == 1));

            // Assert
            Assert.NotNull(newCustomer);
            Assert.Equal(customer.FirstName, newCustomer.FirstName);
            Assert.Equal(1, context.Customers.Count());
            Assert.Equal(Today(), newCustomer.Date);
            Assert.Equal(customer.Email, newCustomer.Email);
        }
    }

    [Fact]
    public void Auto_Mapping_From_ProductDTO_InProduct()
    {
        // Arrange
        using (PurpleOcean context = new PurpleOcean(options))
        {
            // Act
            context.Customers.Add(new Customer
            {
                FirstName = "Денис"
            });

            context.SaveChanges();

            ProductDTO product = new ProductDTO
            {
                Name = "Coconut",
                Description = "Coconut is beautiful!"
            };

            var customer = context.Customers
                .Include(customer => customer.Products)
                .First(customer => customer.CustomerId == 1);

            customer.Products.Add(Mapping.Get<Product, ProductDTO>(product));
            context.SaveChanges();

            var newProduct = Mapping.Get<ProductDTO, Product>(
                context.Products.First(product => product.ProductId == 1));

            var customerDto = Mapping.Get<CustomerDTO, Customer>(
                context.Customers.First(customer => customer.CustomerId == 1));

            // Assert
            Assert.NotNull(newProduct);
            Assert.Equal(customerDto.FirstName, newProduct.Author.FirstName);
        }
    }

    [Fact]
    public void Creating_AConnection_Between_Entities()
    {
        // Assert + Act
        using (PurpleOcean context = new PurpleOcean(options))
        {
            CustomerDTO customerDTO = new CustomerDTO
            {
                FirstName = "Денис",
            };

            context.Customers.Add(Mapping.Get<Customer, CustomerDTO>(customerDTO));
            context.SaveChanges();

            Assert.NotEmpty(context.Customers);
            Assert.Equal(1, context.Customers
                .First(customer => customer.FirstName == "Денис").CustomerId);

            ProductDTO productDTO = new ProductDTO
            {
                Name = "Решение задач",
                Description = "Пример решения задач."
            };

            var customer = context.Customers
                .Include(customer => customer.Products)
                .First(customer => customer.CustomerId == 1);

            customer.Products.Add(Mapping.Get<Product, ProductDTO>(productDTO));
            context.SaveChanges();

            var newProduct = Mapping.Get<ProductDTO, Product>(
                context.Products.First(product => product.ProductId == 1));

            var customerDto = Mapping.Get<CustomerDTO, Customer>(
                context.Customers.First(customer => customer.CustomerId == 1));

            // Assert
            Assert.NotEmpty(context.Products);
            Assert.Equal(customerDto.FirstName, newProduct.Author.FirstName);
        }
    }
}