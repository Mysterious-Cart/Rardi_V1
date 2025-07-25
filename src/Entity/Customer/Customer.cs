
namespace CHKS.Entity;
public record Customer(string Plate,string Name, string Phone = "", string Phone2 = "", string Description = null);
public record CreateCustomerRequest(string Plate, string Name, string Phone = "", string Phone2 = "", string Description = null);