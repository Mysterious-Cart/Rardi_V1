using CHKS.Models.Interface;
using CHKS.Services;

public interface IContainer<ChildrenType>
    where ChildrenType : IItem
{
    abstract Task<ChildrenType> Add(mydbService service, ChildrenType Item);
    abstract Task<ChildrenType> Remove(mydbService service, Guid ItemId);
}