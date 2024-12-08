using MySql.Data.MySqlClient;
using ScheduleApplication.Shared.Domain;
using ScheduleApplication.Shared.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScheduleApplication.Shared.Classes
{
    public class Country
    {
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public AuditInfo AuditInfo { get; set; }
    }

    public interface ICountryRepo
    {
        Task<List<Country>> GetAllCountriesAsync();
    }

    public class CountryRepo : ICountryRepo
    {
        private readonly IDbConnectionFactory _connFact;
        public CountryRepo(IDbConnectionFactory connFact)
        {
            _connFact = connFact;
        }
        public async Task<List<Country>> GetAllCountriesAsync()
        {
            var countries = new List<Country>();

            try
            {
                using (var conn = _connFact.CreateConnection())
                {
                    await conn.OpenAsync();

                    string query = @"SELECT countryId, country, createDate, createdBy, lastUpdate, lastUpdateBy FROM country";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (reader.Read())
                            {
                                Country country = new Country()
                                {
                                    CountryId = reader.GetInt32("countryId"),
                                    CountryName = reader.GetString("country"),
                                    AuditInfo = new AuditInfo
                                    {
                                        CreateDate = reader.GetDateTime("createDate"),
                                        CreatedBy = reader.GetString("createdBy"),
                                        LastUpdate = reader.GetDateTime("lastUpdate"),
                                        LastUpdateBy = reader.GetString("lastUpdateBy")
                                    }
                                };

                                countries.Add(country);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error retrieving countries: {ex.Message}");
            }

            return countries;
        }
    }
}
