using CHKS.Models.mydb;

namespace CHKS.Entity
{
    public class VehicleBuilder
    {
        private int _Id;
        private string _make;
        private string _model;
        private int _year;
        public static VehicleBuilder Empty()
        {
            return new VehicleBuilder();
        }
        public static VehicleBuilder FromModel(Vehicle vehicle)
        {
            return Empty()
                .WithId(vehicle.GetId())
                .WithMake(vehicle.GetMake())
                .WithModel(vehicle.GetModel())
                .WithVersion(vehicle.GetVersion());
        }

        public static Vehicle_Model ToModel(Vehicle vehicle)
        {
            return new Vehicle_Model
            {
                Key = vehicle.GetId(),
                Make = vehicle.GetMake(),
                Model = vehicle.GetModel(),
                Year = vehicle.GetVersion(),
                
            };
        } 

        public static VehicleBuilder Modify(Vehicle vehicle)
        {
            return Empty()
                .WithId(vehicle.GetId())
                .WithMake(vehicle.GetMake())
                .WithModel(vehicle.GetModel())
                .WithVersion(vehicle.GetVersion());
        }

        public VehicleBuilder WithId(int Id)
        {
            _Id = Id;
            return this;
        }
        public VehicleBuilder WithMake(string Maker)
        {
            _make = Maker;
            return this;
        }

        public VehicleBuilder WithModel(string Model)
        {
            _model = Model;
            return this;
        }
        public VehicleBuilder WithVersion(int Version)
        {
            _year = Version;
            return this;
        }

        public Vehicle Build()
        {
            return new Vehicle(_Id, _make, _model, _year);
        }
    }
}