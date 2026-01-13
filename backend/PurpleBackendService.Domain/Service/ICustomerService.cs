using PurpleBackendService.Domain.DTO;

namespace PurpleBackendService.Domain.Service
{
    public interface ICustomerService
    {
        public Task<OperationResult<CustomerDTO>> CreateCustomerAsync(CustomerDTO input);

        public Task<OperationResult<ICollection<CustomerDTO>>> GetCustomers();

        public Task<OperationResult<CustomerDTO>> GetCustomer(long id);

        public Task<OperationResult<CustomerDTO>> ChangeCustomerAsync(long id, CustomerDTO input);
    }
}
