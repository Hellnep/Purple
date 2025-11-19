using Purple.Tests.Utilities;

using Purple.Common.ModelValidator;
using Purple.Common.Database.DTO.Sql;
using Purple.Common.Database.Entity.Sql;

namespace Purple.Tests.Units;

public class ModelValidateTest
{
    [Theory]
    [MemberData(nameof(TestData.TestDataCustomer), MemberType = typeof(TestData))]
    public void Validate_Customer_WithValidData_ReturnsNoValidationErrors(Customer customer)
    {
        // Act
        bool isValid = Validate.TryValidate(customer, out var results);
        
        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }

    [Theory]
    [MemberData(nameof(TestData.TestDataProduct), MemberType = typeof(TestData))]
    public void Validate_Product_WithValidData_ReturnsNoValidationErrors(Product product)
    {
        // Act
        bool isValid = Validate.TryValidate(product, out var results);
            
        // Assert
        Assert.True(isValid);
        Assert.Empty(results);
    }
}