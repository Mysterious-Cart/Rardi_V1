namespace CHKS.Mappers;

using Models;
using Entity;
using System.Linq.Expressions;

public class GroupExpressionMapper
{
    public static Expression<Func<GroupModel, Group>> ToGroup()
    {
        return groupModel => new Group
        {
            Id = groupModel.Id,
            Name = groupModel.Name
        };
    }
}

public class GroupMapper
{
    public static Group ToGroup(GroupModel groupModel)
    {
        return new Group
        {
            Id = groupModel.Id,
            Name = groupModel.Name
        };
    }

    public static IEnumerable<Group> ToGroup(IEnumerable<GroupModel> groupModels)
    {
        return groupModels.Select(ToGroup);
    }
}