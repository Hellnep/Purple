using Purple.Common.ModelValidator;
using Purple.Common.Database.Entity.Sql;

namespace Purple.Tests.Units;

public class ModelValidateTest
{
    [Theory]
    [InlineData("Денис", "hellnep@ya.ru")]
    [InlineData("Никита", "steven@sredadev.com")]
    [InlineData("Саша", "cortezgod.r")]
    public void Validate_Customer(string firstName, string email)
    {
        // Arrange
        Customer newCustomer = new Customer
        {
            FirstName = firstName,
            Email = email
        };

        // Act + Assert
        if (!Validate.TryValidate(newCustomer, out var results))
            Assert.NotEmpty(results);
        else
            Assert.Empty(results);
    }
}