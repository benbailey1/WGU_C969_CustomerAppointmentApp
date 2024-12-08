using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ScheduleApplication.Features.Customers
{
    public class CustomerValidator
    {
        public static List<string> ValidateCustomer(Customer customer)
        {
            var errors = new List<string>();

            // Validate Customer Name
            if (string.IsNullOrWhiteSpace(customer.CustomerName))
            {
                errors.Add("Customer name is required");
            }

            // Validate Customer Address
            if (customer.Address == null)
            {
                errors.Add("Address is required.");
            }
            else
            {
                // Validate essential parts of the address 
                if (string.IsNullOrWhiteSpace(customer.Address.Address1))
                {
                    errors.Add("Street and House Number are required");
                }
                if (string.IsNullOrWhiteSpace(customer.Address.PostalCode))
                {
                    errors.Add("Postal code is required");
                }
                if (customer.Address.CityId <= 0)
                {
                    errors.Add("A valid City is required");
                }
                if (string.IsNullOrWhiteSpace(customer.Address.Phone))
                {
                    errors.Add("Phone number is required");
                }
                else
                {
                    if (!Regex.IsMatch(customer.Address.Phone, @"^[0-9\-]+$"))
                    {
                        errors.Add("Phone number must only contain digits and dashes."))
                    }
                }
            }

            return errors;
        }
    }
}
