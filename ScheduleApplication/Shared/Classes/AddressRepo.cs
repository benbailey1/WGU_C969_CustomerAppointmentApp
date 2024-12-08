using MySql.Data.MySqlClient;
using ScheduleApplication.Shared.Domain;
using ScheduleApplication.Shared.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScheduleApplication.Shared.Classes
{
    public interface IAddressRepo
    {
        Task<List<Address>> GetAllAddressesAsync();
        Task<List<Address>> GetAddressByCustIdAsync(int custId);
        Task<int> AddAddressAsync(Address address);
        Task UpdateAddressAsync(int custId);
        Task DeleteAddressAsync(int custId);

    }

    public class AddressRepo : IAddressRepo
    {
        private readonly IDbConnectionFactory _connFact;

        public AddressRepo(IDbConnectionFactory connFact)
        {
            _connFact = connFact;
        }

        public async Task<List<Address>> GetAllAddressesAsync()
        {
            var addresses = new List<Address>();
            try
            {
                using (var conn = _connFact.CreateConnection())
                {
                    await conn.OpenAsync();

                    string query = @"SELECT addressId, address, address2, cityId, postalCode, phone, createDate, createdBy, lastUpdate, lastUpdateBy FROM address";

                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                Address address = new Address
                                {
                                    // TODO: Can't use colum names: ex Address1 = reader.GetString("address")
                                    //      results in error: CS1503: Argument 1: cannot convert from string to int
                                    AddressId = reader.GetInt32(0), // addressId
                                    Address1 = reader.GetString(1), // address
                                    Address2 = reader.GetString(2), // address2
                                    CityId = reader.GetInt32(3), // cityId
                                    PostalCode = reader.GetString(4), // phone
                                    AuditInfo = new AuditInfo
                                    {
                                        CreateDate = reader.GetDateTime(5), // createDate
                                        CreatedBy = reader.GetString(6), // createdBy
                                        LastUpdate = reader.GetDateTime(7), // lastUpdate
                                        LastUpdateBy = reader.GetString(8), // lastUpdateBy
                                    }
                                };

                                addresses.Add(address);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving addresses: {ex.Message}");
            }
            
            return addresses;
        }

        public Task<int> AddAddressAsync(Address address)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAddressAsync(int custId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Address>> GetAddressByCustIdAsync(int custId)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAddressAsync(int custId)
        {
            throw new NotImplementedException();
        }
    }
}
