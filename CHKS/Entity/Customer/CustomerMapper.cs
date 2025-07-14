using System.Linq.Expressions;
using CHKS.Models;

namespace CHKS.Entity
{
    public static class CustomerExpressionMapper
    {
        public static Expression<Func<CustomerModel, Customer>> ToCustomer() => 
        customer =>
            new Customer(
                customer.PlateNumber,
                customer.Name,
                customer.Phone,
                customer.Phone_2,
                customer.Description
            );

    }
    
    public static class CustomerMapper
    {
        public static Customer ToCustomer(this CustomerModel customer) =>
            new Customer(
                customer.PlateNumber,
                customer.Name,
                customer.Phone,
                customer.Phone_2,
                customer.Description
            );
    }
}