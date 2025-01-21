using CHKS.Models;
using System.Linq.Expressions;


namespace CHKS.Models;
public interface IDbProvider
{
    public Task<IQueryable<T>> GetData<T>(List<string> ToExpand = null) where T : class, IModelClass;
    public Task UpdateData<T, TKey>(T Object,Func<T, TKey> Selector, bool ComfirmExistance = true) where T : class, IModelClass;
    public Task DeleteData<T, TKey>(Func<T, TKey> Selector, TKey key, bool ComfirmExistance = true) where T : class, IModelClass;
}