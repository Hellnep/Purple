using PurpleBackendService.Domain.DTO;

namespace PurpleBackendService.Domain.Service
{
    public interface IUserService
    {
        public Task<OperationResult<UserDTO>> CreateUserAsync(UserDTO input);

        public Task<OperationResult<ICollection<UserDTO>>> GetUsersAsync();

        public Task<OperationResult<UserDTO>> GetUserAsync(long id);

        public Task<OperationResult<UserDTO>> ChangeUserAsync(long id, UserDTO input);
    }
}
