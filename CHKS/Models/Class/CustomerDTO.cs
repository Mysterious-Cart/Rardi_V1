using CHKS.Models.mydb;

namespace CHKS.Models.Class;
public class CustomerDTO
{

    private readonly string _Plate;

    public string Plate => _Plate;

    private readonly string _Phone;

    public string Phone => _Phone;

    public CustomerDTO(string Plate, string Phone)
    {
        _Plate = Plate;
        _Phone = Phone;
    }
}