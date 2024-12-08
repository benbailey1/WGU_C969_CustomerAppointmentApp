using ScheduleApplication.Shared.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScheduleApplication.Features.Customers
{
    public interface ICustomerService
    {
        Task<Result<int>> CreateCustomerAsync(Customer customer);
        Task<Result<List<CustomerResponse>>> GetAllCustomersAsync();
        Task<Result<CustomerResponse>> GetCustomerByIdAsync(int customerId);
        Task<Result<bool>> UpdateCustomerAsync(Customer customer);
        Task<Result<bool>> DeleteCustomerAsync(int customerId);
    }
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

                var validationErrors = CustomerValidator.ValidateCustomer(customer);

                if (validationErrors.Any())
                {
                    var errors = string.Join(", ", validationErrors);
                    // TODO: Implement logging
                    return Result<int>.Failure(errors);
                }

                var customerId = Convert.ToInt32(await _customerRepo.AddCustomerAsync(customer));

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

        public async Task<Result<List<CustomerResponse>>> GetAllCustomersAsync()
        {
            try
            {
                var result = await _customerRepo.GetAllCustomersAsync();

                if (!result.IsSuccess)
                {
                    MessageBox.Show(result.Error);
                    // TODO: IMPLEMENT LOGGING
                    return Result<List<CustomerResponse>>.Failure(result.Error);
                }

                return Result<List<CustomerResponse>>.Success(result.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred while retrieving customers: {ex.Message}");
                // TODO: LOG THIS
                return Result<List<CustomerResponse>>.Failure("An unexpected error occurred while retrieving customers");
            }
        }

        public async Task<Result<CustomerResponse>> GetCustomerByIdAsync(int customerId)
        {
            try
            {
                var result = await _customerRepo.GetCustomerByIdAsync(customerId);

                if (result.IsNotFound)
                {
                    MessageBox.Show($"Customer with ID {customerId} was not found");
                    // TODO: LOG THIS
                    return Result<CustomerResponse>.NotFound($"Customer with ID {customerId} was not found.")
                }

                if (!result.IsSuccess)
                {
                    MessageBox.Show(result.Error);
                    // TODO: LOG THIS
                    return Result<CustomerResponse>.Failure(result.Error);
                }

                return Result<CustomerResponse>.Success(result.Value);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred while retrieving customer: {ex.Message}");
                // TODO: LOG THIS
                return Result<CustomerResponse>.Failure($"An unexpected error occurred while retrieving customer: {ex.Message}")
            }
        }

        public async Task<Result<bool>> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                if (customer == null)
                {
                    // TODO: LOG THIS
                    return Result<bool>.Failure("Customer cannot be null");
                }

                var validationErrors = CustomerValidator.ValidateCustomer(customer);

                if (validationErrors.Any())
                {
                    var errors = string.Join(", ", validationErrors);
                    MessageBox.Show(errors);
                    // TODO: LOG THIS
                    return Result<bool>.Failure(errors);
                }

                var result = await _customerRepo.UpdateCustomerAsync(customer);

                if (result.IsNotFound)
                {
                    MessageBox.Show($"Customer with ID {customer.CustomerId} was not found");
                    // TODO: LOG THIS
                    return Result<bool>.NotFound($"Customer with ID {customer.CustomerId} was not found");
                }

                if (!result.IsSuccess)
                {
                    MessageBox.Show(result.Error);
                    // TODO: LOG THIS
                    return Result<bool>.Failure(result.Error);
                }

                MessageBox.Show($"Customer {customer.CustomerId} updated successfully");
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred while updating customer: {ex.Message}");
                // TODO: LOG THIS
                return Result<bool>.Failure("An unexpected error occurred while updating the customer");
            }
        }

        public async Task<Result<bool>> DeleteCustomerAsync(int customerId)
        {
            //TODO: CHECK IF CUSTOMER HAS ANY EXISTING APPOINTMENTS AND DO NOT ALLOW DELETE IF SO

            try
            {
                var result = await _customerRepo.DeleteCustomerByIdAsync(customerId);

                if (result.IsNotFound)
                {
                    MessageBox.Show($"Customer with ID {customerId} was not found");
                    // TODO: LOG THIS
                    return Result<bool>.NotFound($"Customer with ID {customerId} was not found");
                }

                if (!result.IsSuccess)
                {
                    MessageBox.Show(result.Error);
                    // TODO: LOG THIS
                    return Result<bool>.Failure(result.Error);
                }

                MessageBox.Show($"Customer {customerId} deleted successfully");
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error occurred while deleting customer: {ex.Message}");
                // TODO: LOG THIS
                return Result<bool>.Failure("An unexpected error occurred while deleting the customer");
            }
        }
    }
}
