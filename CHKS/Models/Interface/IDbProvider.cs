using System.Linq.Expressions;


namespace CHKS.Models.Interface;
public interface IDbProvider
{
    public Task CreateData<T>( T Data);
    public Task<IQueryable<T>> GetData<T>(List<string> ToExpand = null) where T : class;
    public Task UpdateData<T, TKey>(T Object, Func<T, TKey> Selector, bool ComfirmExistance = true) where T : class;
    public Task DeleteData<T, TKey>(Func<T, TKey> Selector, TKey key, bool ComfirmExistance = true) where T : class;
}