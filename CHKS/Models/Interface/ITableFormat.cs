using CHKS;

namespace CHKS.Models;

public interface ITableFormat<T>
{
    static abstract Task<T> Create(mydbService service , T Item);
    static abstract Task<bool> Remove(mydbService service, Guid ItemId);
    static abstract Task<T> Update(mydbService service, T Item);
}