namespace Purple.Common.Database.Repository;

public interface IRepository<T>
{
    public Task<T> Add(T input);
    public Task<T> Get(int output);
}
