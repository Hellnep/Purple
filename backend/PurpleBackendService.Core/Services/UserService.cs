using PurpleBackendService.Core.Utility;
using PurpleBackendService.Core.Interfaces.Services;
using PurpleBackendService.Domain.Entity;
using PurpleBackendService.Domain.Interfaces.Repositories;

namespace PurpleBackendService.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult<User>> CreateUserAsync(User user)
        {
            if (user.Email is null)
            {
                return OperationResult<User>.Failure("You need to enter an email address");
            }

            if (await _repository.EmailExists(user.Email))
            {
                return OperationResult<User>.Failure("A user with such an email address already exists");
            }

            var result = await _repository.Add(user);

            return OperationResult<User>
                .Success(user);
        }

        public async Task<OperationResult<User>> GetUserAsync(long userId)
        {
            var user = await _repository.Get(userId);

            if (user is null)
            {
                return OperationResult<User>
                    .Failure($"Customer with ID={userId} not found");
            }

            return OperationResult<User>
                .Success(user);
        }

        public async Task<OperationResult<List<User>>> GetUsersAsync()
        {
            var users = await _repository.Get();

            return OperationResult<List<User>>
                .Success(users as List<User> ?? []);
        }

        public async Task<OperationResult<User>> ChangeUserAsync(long userId, User newData)
        {
            try
            {
                var user = await _repository.Get(userId);

                if (user is null)
                {
                    return OperationResult<User>.Failure("User not found");
                }

                if (newData.Nickname is not null)
                {
                    user.Nickname = newData.Nickname;
                }

                if (newData.Email is not null)
                {
                    user.Email = newData.Email;
                }

                if (newData.Phone is not null)
                {
                    user.Phone = newData.Phone;
                }

                await _repository.Update();

                return OperationResult<User>
                    .Success(user);
            }
            catch (ArgumentNullException exception)
            {
                return OperationResult<User>.Failure(exception.Message);
            }
        }
    }
}
