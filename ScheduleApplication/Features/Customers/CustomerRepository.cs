﻿using MySql.Data.MySqlClient;
using ScheduleApplication.Shared.Classes;
using ScheduleApplication.Shared.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows.Forms;

namespace ScheduleApplication.Features.Customers
{
    public interface ICustomerRepo
    {
        Task<Result<int>> AddCustomerAsync(Customer customer);
        Task<Result<List<CustomerResponse>>> GetAllCustomersAsync();
        Task<Result<CustomerResponse>> GetCustomerByIdAsync(int customerId);
        Task<Result<bool>> UpdateCustomerAsync(Customer customer);
        Task<Result<bool>> DeleteCustomerByIdAsync(int customerId);
    }
    public class CustomerRepository : ICustomerRepo
    {
        private readonly IDbConnectionFactory _connFact;
        public CustomerRepository(IDbConnectionFactory connFact)
        {
            _connFact = connFact;
        }
        public async Task<Result<int>> AddCustomerAsync(Customer customer)
        {
            try
            {
                using (var conn = _connFact.CreateConnection())
                {
                    await conn.OpenAsync();

                    using (var trans = conn.BeginTransaction())
                    {
                        try
                        {
                            string addressQuery = @"INSERT INTO address 
                                    (address, address2, cityId, postalCode, phone, createDate, createdBy, lastUpdate, lastUpdateBy)
                                    VALUES 
                                    (@address, @address2, @cityId, @postalCode, @phone, NOW(), @createdBy, NOW(), @lastUpdateBy);
                                    SELECT LAST_INSERT_ID();";

                            int addressId;
                            using (MySqlCommand addressCmd = new MySqlCommand(addressQuery, conn))
                            {
                                addressCmd.Transaction = trans;
                                var p = addressCmd.Parameters;

                                p.AddWithValue("@address", customer.Address.Address1);
                                p.AddWithValue("@address2", customer.Address.Address2 ?? string.Empty);
                                p.AddWithValue("@cityId", customer.Address.CityId);
                                p.AddWithValue("@postalCode", customer.Address.PostalCode);
                                p.AddWithValue("@phone", customer.Address.Phone);
                                p.AddWithValue("@createdBy", customer.AuditInfo.CreatedBy);
                                p.AddWithValue("@lastUpdateBy", customer.AuditInfo.LastUpdateBy);

                                addressId = Convert.ToInt32(await addressCmd.ExecuteScalarAsync());
                            }

                            string customerQuery = @"INSERT INTO customer
                                    (customerName, addressId, active, createDate, createdBy, lastUpdate, lastUpdateBy)
                                    VALUES
                                    (@customerName, @addressId, @active, NOW(), @createdBy, NOW(), @lastUpdateBy);
                                    SELECT LAST_INSERT_ID();";

                            int customerId;

                            using (MySqlCommand customerCmd = new MySqlCommand(customerQuery, conn))
                            {
                                customerCmd.Transaction = trans;
                                var p = customerCmd.Parameters;

                                p.AddWithValue("@customerName", customer.CustomerName);
                                p.AddWithValue("@addressId", addressId);
                                p.AddWithValue("@active", customer.Active);
                                p.AddWithValue("@createdBy", customer.AuditInfo.CreatedBy);
                                p.AddWithValue("@lastUpdateBy", customer.AuditInfo.LastUpdateBy);

                                customerId = Convert.ToInt32(await customerCmd.ExecuteScalarAsync());
                            }

                            trans.Commit();

                            MessageBox.Show($"Customer {customer.CustomerName} added successfully");
                            return Result<int>.Success(customerId);
                        }
                        catch (Exception ex)
                        {
                            // Rollback both operations if either fail
                            trans.Rollback();
                            throw;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding customer: {ex.Message}");
                return Result<int>.Failure($"Failed to add customer: {ex.Message}");
            }   
        }

        public async Task<Result<List<CustomerResponse>>> GetAllCustomersAsync()
        {
            try
            {
                using (var conn = _connFact.CreateConnection())
                {
                    await conn.OpenAsync();

                    string query = @"SELECT Customer.customerId AS customerId, Customer.customerName AS customerName,
                                     Address.phone AS phone, Address.address AS address, Address.address2 AS address2,
                                     City.city AS city, Address.postalCode AS postalCode,
                                     Country.country AS country
                                     FROM Customer
                                     INNER JOIN Address ON Customer.addressId = Address.addressId
                                     INNER JOIN City ON Address.cityId = City.cityId
                                     INNER JOIN Country ON City.countryId = Country.countryId;";
                                    // ORDER BY Customer.customerName Desc;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        var customers = new List<CustomerResponse>();

                        using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                CustomerResponse customer = new CustomerResponse()
                                {
                                    CustomerId = reader.GetInt32("customerId"),
                                    CustomerName = reader.GetString("customerName"),
                                    Phone = reader.GetString("phone"),
                                    Address1 = reader.GetString("address"),
                                    Address2 = reader.GetString("address2"),
                                    City = reader.GetString("city"),
                                    PostalCode = reader.GetString("postalCode"),
                                    Country = reader.GetString("country")
                                };
                                customers.Add(customer);
                            }
                        }

                        return customers.Any()
                            ? Result<List<CustomerResponse>>.Success(customers)
                            : Result<List<CustomerResponse>>.NotFound("No customers found in the database");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving customers: {ex.Message}");
                return Result<List<CustomerResponse>>.Failure($"Failed to retrieve customers: {ex.Message}");
            }
        }

        public async Task<Result<CustomerResponse>> GetCustomerByIdAsync(int customerId)
        {
            try
            {
                using (var conn = _connFact.CreateConnection())
                {
                    await conn.OpenAsync();

                    string query = @"SELECT Customer.customerId AS customerId, Customer.customerName AS customerName,
                                     Address.phone AS phone, Address.address AS address, Address.address2 AS address2,
                                     City.city AS city, Address.postalCode AS postalCode,
                                     Country.country AS country
                                     FROM Customer
                                     INNER JOIN Address ON Customer.addressId = Address.addressId
                                     INNER JOIN City ON Address.cityId = City.cityId
                                     INNER JOIN Country ON City.countryId = Country.countryId
                                     WHERE Customer.customerId = @customerId;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@customerId", customerId);

                        using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                var customer = new CustomerResponse()
                                {
                                    CustomerId = reader.GetInt32("customerId"),
                                    CustomerName = reader.GetString("customerName"),
                                    Phone = reader.GetString("phone"),
                                    Address1 = reader.GetString("address"),
                                    Address2 = reader.GetString("address2"),
                                    City = reader.GetString("city"),
                                    PostalCode = reader.GetString("postalCode"),
                                    Country = reader.GetString("country")
                                };
                                return Result<CustomerResponse>.Success(customer);
                            }

                            return Result<CustomerResponse>.NotFound($"Customer with ID {customerId} was not found");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving customers: {ex.Message}");
                throw;
            }
        }

        public async Task<Result<bool>> UpdateCustomerAsync(Customer customer)
        {
            try
            {
                using (var conn = _connFact.CreateConnection())
                {
                    await conn.OpenAsync();

                    string query = @"UPDATE customer
                                    SET customerName = @customerName,
                                    addressId = @addressId,
                                    active = @active,
                                    lastUpdate = NOW(),
                                    lastUpdateBy = @lastUpdateBy
                                    WHERE customerId = @customerId;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        var param = cmd.Parameters;

                        param.AddWithValue("@customerId", customer.CustomerId);
                        param.AddWithValue("@customerName", customer.CustomerName);
                        param.AddWithValue("@addressId", customer.Address.AddressId);
                        param.AddWithValue("@active", customer.Active);
                        param.AddWithValue("@lastUpdateBy", customer.AuditInfo.LastUpdateBy);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            return Result<bool>.NotFound($"Customer with ID {customer.CustomerId} was not found.");
                        }

                        return Result<bool>.Success(true);
                    }
                }
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to update customer: {ex.Message}");
            }
        }
        
        public async Task<Result<bool>> DeleteCustomerByIdAsync(int customerId)
        {
            try
            {
                using (var conn = _connFact.CreateConnection())
                {
                    await conn.OpenAsync();

                    string query = @"DELETE FROM customer WHERE customerId = @customerId;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@customerId", customerId);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            return Result<bool>.NotFound($"Customer with ID {customerId} was not found");
                        }

                        return Result<bool>.Success(true);
                    }
                }
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Failed to delete customer: {ex.Message}");
            }
        }
    }
}
