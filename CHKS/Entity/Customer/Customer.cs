
namespace CHKS.Entity;
public class Customer(string plate,string name, string phone,  Vehicle vehicle, string phone2 = null, string description = null)
{
    public readonly string Plate = plate;
    public readonly string Name = name;
    public readonly string Phone = phone;
    public readonly string Phone2 = phone2;
    public readonly string Description = description;
    public readonly Vehicle vehicle = vehicle;

    public string GetPlateNumber() => Plate;
    public string GetName() => Name;
    public string GetPhoneNumber2() => Phone2??"";
    public string GetPhoneNumber() => Phone;
    public Vehicle GetVehicle() => vehicle;

}