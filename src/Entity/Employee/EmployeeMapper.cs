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
        (
            model.Id,
            model.Name,
            model.Group != null ? GroupMapper.ToGroup(model.Group).ToList() : null
        );
    }

}

public class EmployeeExpressionMapper
{
    public static Expression<Func<EmployeeModel, Employee>> ToEmployee()
    {
        return model => new Employee
        (
            model.Id,
            model.Name,
            model.Group != null? GroupMapper.ToGroup(model.Group).ToList() : null
        );
    }
}