using PurpleBackendService.Domain.Entity;

namespace PurpleBackendService.Domain.Repository
{
    public interface IUserRepository
    {
        public Task<User> Add(User input);

        public Task<User?> Get(long id);

        public Task<ICollection<User>> Get();

        public Task<int> Update();

        public Task<bool> EmailExists(string email);
    }
}
