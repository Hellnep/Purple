using PurpleBackendService.Domain.DTO;

namespace PurpleBackendService.Domain.Service
{
    public interface ICustomerService
    {
        public Task<OperationResult<CustomerDTO>> CreateCustomerAsync(CustomerDTO input);

        public Task<OperationResult<ICollection<CustomerDTO>>> GetCustomersAsync();

        public Task<OperationResult<CustomerDTO>> GetCustomerAsync(long id);

        public Task<OperationResult<CustomerDTO>> ChangeCustomerAsync(long id, CustomerDTO input);
    }
}
