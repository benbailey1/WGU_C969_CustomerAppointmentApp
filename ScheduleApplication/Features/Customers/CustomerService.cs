using ScheduleApplication.Shared.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScheduleApplication.Features.Customers
{
    public class CustomerService
    {
        private readonly ICustomerRepo _customerRepo;
        public CustomerService(ICustomerRepo customerRepo) 
        { 
            _customerRepo = customerRepo;
        }

        public async Task<Result<int>> CreateCustomerAsync(Customer customer)
        {
            try
            {
                if (customer == null)
                {
                    // TODO: Implement logging 
                    return Result<int>.Failure("Customer cannot be null");
                }

                var validationErrors = ValidateCustomer(customer);

                if (validationErrors.Any())
                {
                    var errors = string.Join(", ", validationErrors);
                    // TODO: Implement logging
                    return Result<int>.Failure(errors);
                }

                var customerId = await _customerRepo.AddCustomerAsync(customer);

                // TODO: Implement logging
                MessageBox.Show($"Customer created successfully with ID: {customerId}");

                return Result<int>.Success(customerId);

            }
            catch (Exception ex)
            {   // TODO: Implement logging
                MessageBox.Show($"Error occurred while creating customer: {ex.Message}");
                return Result<int>.Failure("An unexpected error occurred while creating the customer");
            }
        }

        private IEnumerable<string> ValidateCustomer(Customer customer) 
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(customer.CustomerName))
            { 
                errors.Add("Customer name is required");
            }

            if (string.IsNullOrWhiteSpace(customer.Address.Phone))
            {
                errors.Add("Phone number cannot be empty.");
            }

            if (!Regex.IsMatch(customer.Address.Phone, @"^[0-9\-]+$"))
            {
                errors.Add("Phone number must only contain digits and dashes."))
            }

            return errors;
        }
    }
}
