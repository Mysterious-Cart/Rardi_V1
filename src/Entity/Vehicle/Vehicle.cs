public class Vehicle(int Id, string Model, string Make, int Year)
{
    private readonly int Id = Id;
    private readonly string Model = Model;
    private readonly string Make = Make;
    private readonly int Year = Year;

    public int GetId() => Id;
    public string GetModel() => Model;
    public string GetMake() => Make;
    public int GetVersion() => Year;
}