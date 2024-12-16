using MySql.Data.MySqlClient;
using ScheduleApplication.Shared.Classes;
using ScheduleApplication.Shared.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ScheduleApplication.Shared.Domain.Cities
{
    public class CityRepository : ICityRepository
    {
        private readonly IDbConnectionFactory _connectionFactory;

        public CityRepository(IDbConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<List<City>> GetAllCitiesAsync()
        {
            var cities = new List<City>();

            try
            {
                using (var connection = _connectionFactory.CreateConnection())
                {
                    await connection.OpenAsync();

                    string query = @"SELECT cityId, city, countryId, createDate, createdBy, lastUpdate, lastUpdateBy FROM city";

                    using (var command = new MySqlCommand(query, connection))
                    {
                        using (var reader = await command.ExecuteReaderAsync())
                        {

                            while (await reader.ReadAsync())
                            {
                                City city = new City()
                                {
                                    CityId = reader.GetInt32(0),
                                    CityName = reader.GetString(1),
                                    CountryId = reader.GetInt32(2),
                                    AuditInfo = new AuditInfo
                                    {
                                        CreateDate = reader.GetDateTime(3),
                                        CreatedBy = reader.GetString(4),
                                        LastUpdate = reader.GetDateTime(5),
                                        LastUpdateBy = reader.GetString(6)
                                    }
                                };
                                cities.Add(city);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Error retrieving cities: {ex.Message}");
            }

            return cities;
        }

        public async Task<int> AddCityAsync(City city)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                string query = @"
                    INSERT INTO city (city, countryId, createDate, createdBy, lastUpdate, lastUpdateBy) 
                    VALUES (@CityName, @CountryId, @CreateDate, @CreatedBy, @LastUpdate, @LastUpdateBy);
                    SELECT LAST_INSERT_ID();";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CityName", city.CityName);
                    command.Parameters.AddWithValue("@CountryId", city.CountryId);
                    command.Parameters.AddWithValue("@CreateDate", city.AuditInfo.CreateDate);
                    command.Parameters.AddWithValue("@CreatedBy", city.AuditInfo.CreatedBy);
                    command.Parameters.AddWithValue("@LastUpdate", city.AuditInfo.LastUpdate);
                    command.Parameters.AddWithValue("@LastUpdateBy", city.AuditInfo.LastUpdateBy);

                    return Convert.ToInt32(await command.ExecuteScalarAsync());
                }
            }
        }

        public async Task<City> GetCityByIdAsync(int cityId)
        {
            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();

                string query = @"
                    SELECT cityId, city, countryId, 
                           createDate, createdBy, lastUpdate, lastUpdateBy 
                    FROM city 
                    WHERE cityId = @CityId";

                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@CityId", cityId);

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {

                            City city = new City()
                            {
                                CityId = reader.GetInt32(0),
                                CityName = reader.GetString(1),
                                CountryId = reader.GetInt32(2),
                                AuditInfo = new AuditInfo
                                {
                                    CreateDate = reader.GetDateTime(3),
                                    CreatedBy = reader.GetString(4),
                                    LastUpdate = reader.GetDateTime(5),
                                    LastUpdateBy = reader.GetString(6)
                                }
                            };
                            return city;
                        }
                    }
                }
            }

            return null;
        }

        public Task UpdateCityAsync(City city)
        {
            throw new NotImplementedException();
        }

        public Task DeleteCityAsync(int cityId)
        {
            throw new NotImplementedException();
        }
    }
}
