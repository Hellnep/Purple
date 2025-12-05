namespace PurpleBackendService.Domain.Repository
{
    public interface IRepository<T>
    {
        public Task<T> Add(T input);
        public T Get(long id);
        public ICollection<T> Get();
        public Task<int> Update();
    }    
}