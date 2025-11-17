using Purple.Common.ModelValidator;
using Purple.Common.Database.DTO.Sql;
using Purple.Common.Database.Entity.Sql;

namespace Purple.Tests.Units;

public class ModelValidateTest
{
    [Theory]
    [InlineData("Денис", "hellnep@ya.ru")]
    [InlineData("Никита", "steven@sredadev.com")]
    [InlineData("Саша", "cortezgod.r")]
    [InlineData("Саша", null)]
    public void Validate_Customer(string firstName, string? email)
    {
        // Arrange
        Common.Database.Entity.Sql.Customer customer = new Common.Database.Entity.Sql.Customer
        {
            FirstName = firstName,
            Email = email
        };

        // Act + Assert
        if (!Validate.TryValidate(customer, out var results))
            Assert.NotEmpty(results);
        else
            Assert.Empty(results);
    }

    [Theory]
    [InlineData("Денис", "hellnep@ya.ru")]
    [InlineData("Никита", "steven@sredadev.com")]
    [InlineData("Саша", "cortezgod.r")]
    [InlineData("Саша", null)]
    public void Validate_CustomerDTO(string firstName, string? email)
    {
        // Arrange
        Common.Database.DTO.Sql.CustomerDTO customer = new Common.Database.DTO.Sql.CustomerDTO
        {
            FirstName = firstName,
            Email = email
        };

        // Act + Assert
        if (!Validate.TryValidate(customer, out var results))
            Assert.NotEmpty(results);
        else
            Assert.Empty(results);
    }

    [Theory]
    [InlineData("Денис", "Титульный лист", "Описание")]
    public void Validate_Product(string name, string title, string description)
    {
        // Arrange
        Customer customer = new Customer
        {
            CustomerId = 1,
            FirstName = name
        };

        // Assert + Act
        Product productOne = new Product
        {
            Name = title,
            Description = description,
            AuthorRefId = customer.CustomerId
        };

        Assert.NotEqual(-1, productOne.AuthorRefId);

        if (!Validate.TryValidate(productOne, out var result))
            Assert.NotEmpty(result);
        else
            Assert.Empty(result);

        ProductDTO productTwo = new ProductDTO
        {
            Name = title,
            Description = description
        };

        if (!Validate.TryValidate(productTwo, out result))
            Assert.NotEmpty(result);
        else
            Assert.Empty(result);
    }
}