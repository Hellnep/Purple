using Microsoft.EntityFrameworkCore;

using PurpleBackendService.Domain.Entity;
using PurpleBackendService.Domain.Repository;
using PurpleBackendService.Infrastructure.Sqlite;

namespace PurpleBackendService.Core.Repository
{
    public class UserRepository(PurpleOcean repository)
        : Repository(repository), IUserRepository
    {
        ///<summary>
        ///Add user to database
        ///</summary>
        ///<param name="user">User to add</param>
        ///<returns>
        ///The result of adding a user
        ///</returns>
        public Task<User> Add(User user)
        {
            _repository.Users.Add(user);
            return _repository
                .SaveChangesAsync()
                .ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        return user;
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to save changes");
                    }
                });
        }

        public Task<User?> Get(long userId) => _repository.Users
            .Include(user => user.Products)
            .FirstOrDefaultAsync(user => user.Id == userId);

        ///<summary>
        ///Check if email exists in database
        ///</summary>
        ///<param name="email">Email to check</param>
        ///<returns>
        ///The result of checking the existence an email
        ///</returns>
        public Task<bool> EmailExists(string email) => _repository.Users
            .AnyAsync(user => string.Equals(user.Email, email));

        ///<summary>
        ///Update datas in database
        ///</summary>
        public Task<int> Update() => _repository.SaveChangesAsync();

        ///<summary>
        ///Get all users from database
        ///</summary>
        ///<returns>List of users</returns>
        public Task<ICollection<User>> Get() => Task.FromResult(_repository.Users
                .Include(user => user.Products)
                .ToList() as ICollection<User>
            );
    }
}
