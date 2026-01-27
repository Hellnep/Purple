using PurpleBackendService.Domain.Entity;

namespace PurpleBackendService.Core.Interfaces.Services
{
    public interface IUserService
    {
        public Task<OperationResult<User>> CreateUserAsync(User input);

        public Task<OperationResult<List<User>>> GetUsersAsync();

        public Task<OperationResult<User>> GetUserAsync(long id);

        public Task<OperationResult<User>> ChangeUserAsync(long id, User input);
    }
}
