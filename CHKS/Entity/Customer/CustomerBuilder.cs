using CHKS.Extras.Class.DTOs;
using CHKS.Models.mydb;

namespace CHKS.Entity
{

    public class CustomerBuilder : IEntityBuilder<Customer,Customer_Model>
    {
        private string _plate_number;
        private string _phone_number = "";
        private string _name = "";
        private string _phone_number_2 = null;
        private string _description = null;
        private VehicleBuilder _vehicle = VehicleBuilder.Empty();

        public static Customer_Model ToModel(Customer customer)
        {
            return new Customer_Model
            {
                Plate = customer.GetPlateNumber(),
                Phone = customer.GetPhoneNumber(),
                Phone_2 = customer.GetPhoneNumber2(),
                Name = customer.GetName(),
                Vehicle_Id = customer.GetVehicle().GetId(),
            };
        }
        public static CustomerBuilder Modify(Customer customer)
        {
            return
                Empty()
                    .WithPlateNumber(customer.Plate)
                    .WithName(customer.Name)
                    .WithPhoneNumbers(customer.Phone, customer.Phone2);
        }
        public static IEntityBuilder<Customer, Customer_Model> FromModel(Customer_Model customer)
        {
            return
                Empty()
                    .WithPlateNumber(customer.Plate)
                    .WithName(customer.Name)
                    .WithPhoneNumbers(customer.Phone, customer.Phone_2);
        }

        public static CustomerBuilder Empty()
        {
            return new CustomerBuilder();
        }
        public CustomerBuilder WithPlateNumber(string plateNumber)
        {
            _plate_number = plateNumber.Replace(" ", "").ToUpper();
            return this;
        }

        public CustomerBuilder WithName(string Name = "")
        {
            this._name = Name.ToUpper();
            return this;
        }

        public CustomerBuilder WithPhoneNumbers(string phoneNumber, string phoneNumber2 = null)
        {
            _phone_number = string.IsNullOrEmpty(phoneNumber)? "" : phoneNumber;
            _phone_number_2 = string.IsNullOrEmpty(phoneNumber2) ? null : phoneNumber2;
            return this;
        }
        

        public CustomerBuilder WithVehicle(Action<VehicleBuilder> vehicle)
        {
            vehicle(_vehicle);
            return this;
        }
        public CustomerBuilder WithVehicle(Vehicle vehicle)
        {
            _vehicle = VehicleBuilder.Modify(vehicle);
            return this;
        }
        
        public CustomerBuilder WithDescription(string Description)
        {
            _description = Description;
            return this;
        }

        public Customer Build()
        {
            return new Customer(_plate_number, _name, _phone_number, _vehicle.Build(), _phone_number_2, _description);
        }
    }
}