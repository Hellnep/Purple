using PurpleBackendService.Domain.DTO;

namespace PurpleBackendService.Domain.Service
{
    public interface ICustomerService
    {
        public Task<OperationResult<UserDTO>> CreateCustomerAsync(UserDTO input);

        public Task<OperationResult<ICollection<UserDTO>>> GetCustomersAsync();

        public Task<OperationResult<UserDTO>> GetCustomerAsync(long id);

        public Task<OperationResult<UserDTO>> ChangeCustomerAsync(long id, UserDTO input);
    }
}
