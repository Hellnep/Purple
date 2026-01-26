using PurpleBackendService.Domain.DTO;
using PurpleBackendService.Domain.Entity;
using PurpleBackendService.Domain.Repository;
using PurpleBackendService.Domain.Service;
using PurpleBackendService.Core.Utility;

namespace PurpleBackendService.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;

        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult<UserDTO>> CreateUserAsync(UserDTO input)
        {
            var user = Mapping
                .Get<User, UserDTO>(input);

            if (user.Email is null)
            {
                return OperationResult<UserDTO>.Failure("You need to enter an email address");
            }

            if (await _repository.EmailExists(user.Email))
            {
                return OperationResult<UserDTO>.Failure("A user with such an email address already exists");
            }

            var result = await _repository.Add(user);

            return OperationResult<UserDTO>
                .Success(Mapping.Get<UserDTO, User>(result));
        }

        public async Task<OperationResult<UserDTO>> GetUserAsync(long userId)
        {
            var user = await _repository.Get(userId);

            if (user is null)
            {
                return OperationResult<UserDTO>
                    .Failure($"Customer with ID={userId} not found");
            }

            return OperationResult<UserDTO>
                .Success(Mapping.Get<UserDTO, User>(user));
        }

        public async Task<OperationResult<ICollection<UserDTO>>> GetUsersAsync()
        {
            var users = await _repository.Get();
            var result = new List<UserDTO>();

            foreach (var customer in users)
            {
                result.Add(Mapping.Get<UserDTO, User>(customer));
            }

            return OperationResult<ICollection<UserDTO>>
                .Success(result);
        }

        public async Task<OperationResult<UserDTO>> ChangeUserAsync(long userId, UserDTO input)
        {
            try
            {
                var newData = Mapping.Get<User, UserDTO>(input);
                var user = await _repository.Get(userId);

                if (user is null)
                {
                    return OperationResult<UserDTO>.Failure("User not found");
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

                return OperationResult<UserDTO>
                    .Success(Mapping.Get<UserDTO, User>(user));
            }
            catch (ArgumentNullException exception)
            {
                return OperationResult<UserDTO>.Failure(exception.Message);
            }
        }
    }
}
