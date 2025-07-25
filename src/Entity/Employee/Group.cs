namespace CHKS.Entity
{
    public class Group
    {

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Employee> Employee { get; set; }
        
    }
}