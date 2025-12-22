using PurpleBackendService.Domain.DTO;

namespace PurpleBackendService.Domain.Service
{
    public interface ICustomerService
    {
        public Task<OperationResult<CustomerDTO>> CreateCustomerAsync(CustomerDTO input);

        public OperationResult<ICollection<CustomerDTO>> GetCustomers();

        public OperationResult<CustomerDTO> GetCustomer(long id);

        public Task<OperationResult<CustomerDTO>> ChangeCustomerAsync(long id, CustomerDTO input);
    }
}
