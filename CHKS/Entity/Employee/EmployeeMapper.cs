namespace CHKS.Mappers;

using Entity;
using Models;
using System.Linq.Expressions;

public class EmployeeMapper
{
    public static Employee ToEmployee(EmployeeModel model)
    {
        if (model == null) return null;

        return new Employee
        {
            Id = model.Id,
            Name = model.Name
        };
    }

}

public class EmployeeExpressionMapper
{
    public static Expression<Func<EmployeeModel, Employee>> ToEmployee()
    {
        return model => new Employee
        {
            Id = model.Id,
            Name = model.Name,
            Group = model.Group != null? GroupMapper.ToGroup(model.Group).ToList() : null
        };
    }
}