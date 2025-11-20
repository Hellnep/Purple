namespace Purple.Common.Database.Repository;

public interface IRepository<T>
{
    public Task<T> Add(T input);
    public T Get(long id);

    public Task<int> Update();
}
