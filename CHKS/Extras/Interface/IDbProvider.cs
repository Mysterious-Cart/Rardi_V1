using System.Linq.Expressions;


namespace CHKS.Models.Interface;
public interface IDbProvider : IAsyncDisposable
{
    public Task CreateData<T>(T Data) where T : class;
    public Task<IQueryable<T>> GetData<T>(List<string> ToExpand = null) where T : class;
    public Task UpdateData<T, TKey>(T Object, Func<T, TKey> Selector) where T : class;
    public Task UpdateData<TEntity, TKey>(Action<TEntity> Object, TKey key)
    where TEntity : class;
    public Task Transaction(Func<Task> action, CancellationToken token);
    public Task DeleteData<T, TKey>(Func<T, TKey> Selector, TKey key) where T : class;
    public Task BulkInsert<T>(List<T> Data) where T : class;
    public IDbProvider OnDifferentDbContext();
}