using PurpleBackendService.Core.DTOs.User;

namespace PurpleBackendService.Core.Interfaces.Services
{
    public interface IUserService
    {
        public Task<OperationResult<UserDTO>> CreateUserAsync(UserDTO input);

        public Task<OperationResult<ICollection<UserDTO>>> GetUsersAsync();

        public Task<OperationResult<UserDTO>> GetUserAsync(long id);

        public Task<OperationResult<UserDTO>> ChangeUserAsync(long id, UserDTO input);
    }
}
