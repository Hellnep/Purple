using PurpleBackendService.Infrastructure.Sqlite;

namespace PurpleBackendService.Core.Repository
{
    public abstract class Repository
    {
        protected PurpleOcean _repository;

        protected Repository(PurpleOcean repository)
        {
            _repository = repository;
        }
    }
}