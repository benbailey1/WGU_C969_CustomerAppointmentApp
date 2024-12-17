using MySql.Data.MySqlClient;
using ScheduleApplication.Features.Appointments.Models;
using ScheduleApplication.Shared.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScheduleApplication.Features.Login
{
    public interface IUserLoginRepository
    {
        Task<UserLoginModel> GetUserByCredentialsAsync(string username, string hashedPassword);
        Task<List<AppointmentModel>> GetUpcomingAppointmentsAsync(int userId, DateTime startTime, DateTime endTime);
    }

    public class UserLoginRepository : IUserLoginRepository
    {
        private readonly IDbConnectionFactory _dbCon;

        public UserLoginRepository(IDbConnectionFactory dbCon)
        {
            _dbCon = dbCon;
        }
        public async Task<List<AppointmentModel>> GetUpcomingAppointmentsAsync(int userId, DateTime startTime, DateTime endTime)
        {
            try
            {
                using (var conn = _dbCon.CreateConnection())
                {
                    await conn.OpenAsync();

                    string query = @"SELECT a.appointmentId, c.customerName, a.start, a.type
                               FROM appointment a
                               INNER JOIN customer c ON a.customerId = c.customerId
                               WHERE a.userId = @userId
                               AND a.start BETWEEN @startTime AND @endTime
                               ORDER BY a.start;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@userId", userId);
                        cmd.Parameters.AddWithValue("@startTime", startTime);
                        cmd.Parameters.AddWithValue("@endTime", endTime);

                        var appointments = new List<AppointmentModel>();
                        using (MySqlDataReader reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                appointments.Add(new AppointmentModel
                                {
                                    AppointmentId = reader.GetInt32("appointmentId"),
                                    CustomerName = reader.GetString("customerName"),
                                    Start = reader.GetDateTime("start"),
                                    Type = reader.GetString("type")
                                });
                            }
                        }
                        return appointments;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error accessing appointment data");
                throw new Exception("Error accessing appointment data", ex);
            }
        }

        public async Task<UserLoginModel> GetUserByCredentialsAsync(string username, string hashedPassword)
        {
            try
            {
                using (var conn = _dbCon.CreateConnection())
                {
                    await conn.OpenAsync();

                    string query = @"SELECT userId, userName FROM user WHERE userName = @username AND password = @password;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@username", username);
                        cmd.Parameters.AddWithValue("@password", hashedPassword);

                        using (var reader = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                return new UserLoginModel
                                {
                                    UserId = reader.GetInt32("userId"),
                                    UserName = reader.GetString("username")
                                };
                            }
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error accessing user data");
                throw new Exception("Error accessing user data", ex);
            }
        }
    }
}
