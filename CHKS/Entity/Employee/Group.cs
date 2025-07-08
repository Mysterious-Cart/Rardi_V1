namespace CHKS.Entity
{
    public class Group
    {

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Employee> Employee { get; set; }
        
        public static Group FromGroupModel(Models.GroupModel group)
        {
            return new Group
            {
                Id = group.Id,
                Name = group.Name,
                Employee = group.Employee.Select(e => new Employee
                {
                    Id = e.Id,
                    Name = e.Name
                }).ToList()
            };
        }
    }
}