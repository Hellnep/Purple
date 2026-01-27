using PurpleBackendService.Infrastructure.Sqlite;

namespace PurpleBackendService.Infrastucture.Repository
{
    ///<summary>
    /// Base class for all repositories
    /// </summary>
    public abstract class Repository(PurpleOcean repository)
    {
        protected PurpleOcean _repository = repository;
    }
}
