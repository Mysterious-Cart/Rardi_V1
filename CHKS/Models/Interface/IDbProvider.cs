using CHKS.Models;

namespace CHKS.Models;
public interface IDbProvider
{
    public Task<IQueryable<T>> GetData<T>() where T : class, IModelClass;
}