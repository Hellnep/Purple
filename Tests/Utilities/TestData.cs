using Purple.Common.Database.DTO.Sql;
using Purple.Common.Database.Entity.Sql;

namespace Purple.Tests.Utilities;

public static class TestData
{
    public static IEnumerable<object[]> TestDataCustomer()
    {
        yield return new object[] 
        { 
            new Customer 
            { 
                FirstName = "Hellnep", 
                Email = "example@test.com",
                Products = new List<Product>()
            }
        };

        yield return new object[] 
        { 
            new Customer
            {
                FirstName = "Никита",
                Email = "example@test.com",
                Products = new List<Product>()
                {
                    new Product
                    {
                        Name = "Tomato",
                        Description = "It is so good"
                    }
                }
            }
        };

        yield return new object[] 
        { 
            new Customer
            {
                FirstName = "Hellnep",
                Email = "test@example.com",
                Phone = "8(999)9999999",
                Products = new List<Product>()
            }
        };
    }

    public static IEnumerable<object[]> TestDataCustomerDTO()
    {
        yield return new object[] 
        { 
            new CustomerDTO 
            { 
                FirstName = "Hellnep", 
                Email = "example@google.com",
            }
        };

        yield return new object[] 
        { 
            new CustomerDTO
            {
                FirstName = "Никита",
                Email = "example@test.com",
            }
        };

        yield return new object[] 
        { 
            new CustomerDTO
            {
                FirstName = "Hellnep",
                Email = "test@example.com",
                Phone = "8(999)9999999"
            }
        };
    }

    public static IEnumerable<object[]> TestDataProduct()
    {
        yield return new object[]
        {
            new Product
            {
                Name = "Melon",
                Description = "It is a nice fruit",
                AuthorRefId = 1,
                Author = new Customer
                {
                    FirstName = "Hellnep",
                    Email = "example@google.com"
                }
            }
        };

        yield return new object[]
        {
            new Product
            {
                Name = "Картошка",
                AuthorRefId = 1,
                Author = new Customer
                {
                    FirstName = "Hellnep",
                    Email = "example@google.com"
                }
            }
        };
    }
}