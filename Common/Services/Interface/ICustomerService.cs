using Purple.Common.Database.DTO.Sql;

namespace Purple.Common.Services.Interface;

public interface ICustomerService
{
    public Task<OperationResult<CustomerDTO>> CreateCustomerAsync(CustomerDTO input);
    public Task<OperationResult<CustomerDTO>> GetCustomerAsync(long id);
    public Task<OperationResult<CustomerDTO>> ChangeCustomerAsync(long id, CustomerDTO input);
}