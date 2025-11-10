using Purple.Common.ModelValidator;
using Purple.Common.Database.Entity.Sql;
using Purple.Common.Database.DTO.Sql;

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
        Customer customer = new Customer
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
        CustomerDTO customer = new CustomerDTO
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
}