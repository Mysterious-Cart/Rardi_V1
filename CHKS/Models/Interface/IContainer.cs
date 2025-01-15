using CHKS.Models;
using CHKS;

public interface IContainer<ChildrenType>
    where ChildrenType : IItem
{
    abstract Task<ChildrenType> Add(mydbService service, ChildrenType Item);
    abstract Task<ChildrenType> Remove(mydbService service, Guid ItemId);
}