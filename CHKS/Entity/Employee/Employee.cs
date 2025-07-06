namespace CHKS.Entity
{
    public class Employee
    {

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Group> Group { get; set; }
        
        public static Employee FromEmployeeModel(Models.Employee employee)
        {
            return new Employee
            {
                Id = employee.Id,
                Name = employee.Name,
                Group = employee.Group.Select(g => new Group
                {
                    Id = g.Id,
                    Name = g.Name
                }).ToList()
            };
        }
    }
}