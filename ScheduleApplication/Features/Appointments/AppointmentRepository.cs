using MySql.Data.MySqlClient;
using MySqlX.XDevAPI.Common;
using ScheduleApplication.Features.Appointments.Models;
using ScheduleApplication.Shared.Classes;
using ScheduleApplication.Shared.Infrastructure.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScheduleApplication.Features.Appointments
{
    public interface IAppointmentRepository
    {
        Task<AppointmentResult<int>> AddAppointmentAsync(AppointmentModel appt);
        Task<AppointmentResult<List<AppointmentModel>>> GetAllAppointmentsAsync();
        Task<AppointmentResult<bool>> UpdateAppointmentAsync(AppointmentModel appt);
        Task<AppointmentResult<bool>> DeleteAppointmentAsync(int id);
    }
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IDbConnectionFactory _dbCon;
        public AppointmentRepository(IDbConnectionFactory dbCon)
        {
            _dbCon = dbCon;
        }

        public async Task<AppointmentResult<int>> AddAppointmentAsync(AppointmentModel appt)
        {
            try
            {
                using (var conn = _dbCon.CreateConnection())
                {
                    await conn.OpenAsync();

                    string query = @"INSERT INTO appointment (customerId, userId, title, description, location, contact, type, url, start, end, createDate, createdBy, lastUpdate, lastUpdateBy)
                                     VALUES (@customerId, @userId, @title, @description, @location, @contact, @type, @url, @start, @end, NOW(), @createdBy, NOW(), @lastUpdateBy)";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        var p = cmd.Parameters;

                        p.AddWithValue("@customerId", appt.CustomerId);
                        p.AddWithValue("@userId", appt.UserId);
                        p.AddWithValue("@title", appt.Title);
                        p.AddWithValue("@description", appt.Description);
                        p.AddWithValue("@location", appt.Location);
                        p.AddWithValue("@contact", appt.Contact);
                        p.AddWithValue("@type", appt.Type);
                        p.AddWithValue("@url", appt.Url);
                        p.AddWithValue("@start", appt.Start);
                        p.AddWithValue("@end", appt.End);
                        p.AddWithValue("@createDate", appt.AuditInfo.CreateDate);
                        p.AddWithValue("@createdBy", appt.AuditInfo.CreatedBy);
                        p.AddWithValue("@lastUpdate", appt.AuditInfo.LastUpdate);
                        p.AddWithValue("@lastUpdateBy", appt.AuditInfo.LastUpdateBy);

                        var newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());
                        MessageBox.Show($"Appointment {newId} added successfully");
                        return AppointmentResult<int>.Success(newId);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding appointment: {ex.Message}");
                return AppointmentResult<int>.Failure($"Failed to add appointment: {ex.Message}");
            }
        }

        public async Task<AppointmentResult<List<AppointmentModel>>> GetAllAppointmentsAsync()
        {
            try
            {
                using (var conn = _dbCon.CreateConnection())
                {
                    await conn.OpenAsync();

                    string query = @"SELECT appointmentId, customerId, userId, title, description, location, contact, type, url, start, end, createDate, createdBy, lastUpdate, lastUpdateBy FROM appointment";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        var appts = new List<AppointmentModel>();

                        using (MySqlDataReader r = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            while (r.Read())
                            {
                                AppointmentModel apptMod = new AppointmentModel()
                                {
                                    AppointmentId = r.GetInt32("appointmentId"),
                                    CustomerId = r.GetInt32("customerId"),
                                    UserId = r.GetInt32("userId"),
                                    Title = r.GetString("title"),
                                    Description = r.GetString("description"),
                                    Location = r.GetString("location"),
                                    Contact = r.GetString("contact"),
                                    Type = r.GetString("type"),
                                    Url = r.GetString("url"),
                                    Start = TimeZoneInfo.ConvertTimeFromUtc(r.GetDateTime("start"), TimeZoneInfo.Local),
                                    End = TimeZoneInfo.ConvertTimeFromUtc(r.GetDateTime("end"), TimeZoneInfo.Local),
                                    AuditInfo = new AuditInfo
                                    {
                                        CreateDate = r.GetDateTime("createDate"),
                                        CreatedBy = r.GetString("createdBy"),
                                        LastUpdate = r.GetDateTime("lastUpdate"),
                                        LastUpdateBy = r.GetString("lastUpdateBy")
                                    }
                                };
                                appts.Add(apptMod);
                            }
                        }

                        return appts.Any()
                            ? AppointmentResult<List<AppointmentModel>>.Success(appts)
                            : AppointmentResult<List<AppointmentModel>>.NotFound("No appointments found in the database.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving appointments: {ex.Message}");
                return AppointmentResult<List<AppointmentModel>>.Failure($"Error retrieving appointments {ex.Message}");
            }
        }

        public async Task<AppointmentResult<bool>> UpdateAppointmentAsync(AppointmentModel appt)
        {
            try
            {
                using (var conn = _dbCon.CreateConnection())
                {
                    await conn.OpenAsync();

                    string query = @"UPDATE appointment SET customerId = @customerId, userId = @userId, title = @title, description = @description, location = @location, contact = @contact, type = @type, url = @url, 
                                                 start = @start, end = @end, lastUpdate = NOW(), lastUpdateBy = @lastUpdateBy WHERE appointmentId = @appointmentId";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        var p = cmd.Parameters;

                        p.AddWithValue(@"appointmentId", appt.AppointmentId);
                        p.AddWithValue("@customerId", appt.CustomerId);
                        p.AddWithValue("@userId", appt.UserId);
                        p.AddWithValue("@title", appt.Title);
                        p.AddWithValue("@description", appt.Description);
                        p.AddWithValue("@location", appt.Location);
                        p.AddWithValue("@contact", appt.Contact);
                        p.AddWithValue("@type", appt.Type);
                        p.AddWithValue("@url", appt.Url);
                        p.AddWithValue("@start", appt.Start);
                        p.AddWithValue("@end", appt.End);
                        p.AddWithValue("@lastUpdate", appt.AuditInfo.LastUpdate);
                        p.AddWithValue("@lastUpdateBy", appt.AuditInfo.LastUpdateBy);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            return AppointmentResult<bool>.NotFound($"Appointment with ID: {appt.AppointmentId} was not found.");
                        }

                        return AppointmentResult<bool>.Success(true);
                    }
                }
            }
            catch (Exception ex)
            {
                return AppointmentResult<bool>.Failure($"Failed to update appointment: {ex.Message}");
            }
        }

        public async Task<AppointmentResult<bool>> DeleteAppointmentAsync(int id)
        {
            try
            {
                using (var conn = _dbCon.CreateConnection())
                {
                    await conn.OpenAsync();

                    string query = @"DELETE FROM appointment WHERE appointmentId = @appointmentId;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@appointmentId", id);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            return AppointmentResult<bool>.NotFound($"Appointment with ID {id} was not found ");
                        }

                        return AppointmentResult<bool>.Success(true);
                    }
                }
            }
            catch (Exception ex)
            {
                return AppointmentResult<bool>.Failure($"Failed to delete customer: {ex.Message}");
            }
        }
    }
}
