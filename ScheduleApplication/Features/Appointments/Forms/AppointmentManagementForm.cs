using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using ScheduleApplication.Shared.Infrastructure.Database;

namespace ScheduleApplication.Features.Appointments
{
    public partial class AppointmentManagementForm : Form
    {
        private readonly IDbConnectionFactory _dbConFact;
        private readonly int _loggedInUserId;
        private Label loadingLabel;
        public AppointmentManagementForm(IDbConnectionFactory dbConFact, int loggedInUserId)
        {
            InitializeComponent();
            _dbConFact = dbConFact;
            _loggedInUserId = loggedInUserId;

            loadingLabel = new Label
            {
                Text = "Loading data...",
                AutoSize = true,
                Location = new Point(10, 10),
                Visible = false
            };
            this.Controls.Add(loadingLabel);

            this.Load += AppointmentForm_Load;
        }

        #region Form Setup
        private async void AppointmentForm_Load(object sender, EventArgs e)
        {
            loadingLabel.Visible = true;
            await InitializeAsync();
            loadingLabel.Visible = false;
        }

        private async Task InitializeAsync()
        {
            try
            {
                await Task.WhenAll(
                        GetAppointmentsAsync(),
                        LoadCustomersAsync()
                    );
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
        }

        private async Task GetAppointmentsAsync()
        {
            try
            {
                using (var conn = _dbConFact.CreateConnection())
                {
                    await conn.OpenAsync();

                    string query = @"SELECT appointmentId, customerId, type, start, end FROM appointment";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {

                        DataTable dt = new DataTable();

                        using (MySqlDataReader r = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(r);

                            foreach (DataRow row in dt.Rows)
                            {
                                row["start"] = ConvertToLocalTime((DateTime)row["start"]);
                                row["end"] = ConvertToLocalTime((DateTime)row["end"]);
                            }

                            dataGridViewAppointments.DataSource = dt;

                            dataGridViewAppointments.Columns["start"].Width = 150;
                            dataGridViewAppointments.Columns["end"].Width = 150;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error retrieving appointments: {ex.Message}");
            }
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                using (var conn = _dbConFact.CreateConnection())
                {
                    await conn.OpenAsync();

                    string query = @"SELECT customerId, customerName FROM customer";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {

                        DataTable dt = new DataTable();

                        using (MySqlDataReader r = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                        {
                            dt.Load(r);

                            DataRow blankRow = dt.NewRow();
                            blankRow["customerId"] = -1;
                            blankRow["customerName"] = "Select Customer";
                            dt.Rows.InsertAt(blankRow, 0);

                            comboBoxCustomer.DisplayMember = "customerName";
                            comboBoxCustomer.ValueMember = "customerId";
                            comboBoxCustomer.DataSource = dt;
                            
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}");
            }
        }

        #endregion


        #region UI Interactions

        private async void btnAddAppointment_Click(object sender, EventArgs e)
        {
            string type = txtType.Text.Trim();
            DateTime start = ConvertToUtcTime(dateTimePickerStart.Value);
            DateTime end = ConvertToUtcTime(dateTimePickerEnd.Value);
            int customerId = (int)comboBoxCustomer.SelectedValue;

            if (comboBoxCustomer.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer.");
                return;
            }

            if (customerId == -1)
            {
                MessageBox.Show("Please select a valid customer.");
                return;
            }

            if (string.IsNullOrEmpty(type))
            {
                MessageBox.Show("Please enter an appointment type.");
                return;
            }

            if (start >= end)
            {
                MessageBox.Show("The start time must be earlier than the end time.");
                return;
            }

            if (!IsInBusinessHours(start, end))
            {
                MessageBox.Show("Appointments must be scheduled between 9:00 AM and 5:00 PM, Monday to Friday (EST).");
                return;
            }

            if (await DoAppointmentsOverlap(customerId, start, end))
            {
                MessageBox.Show("The appointment overlaps with an existing appointment. Please choose another time.");
                return;
            }


            try
            {
                using (var conn = _dbConFact.CreateConnection())
                {

                    string query =
                        "INSERT INTO appointment (customerId, userId, title, description, location, contact, type, url, start, end, createDate, createdBy, lastUpdate, lastUpdateBy) " +
                        "VALUES (@customerId, @userId, @title, @description, @location, @contact, @type, @url, @start, @end, NOW(), @createdBy, NOW(), @lastUpdateBy);";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        var p = cmd.Parameters;

                        p.AddWithValue("@customerId", customerId);
                        p.AddWithValue("@userID", _loggedInUserId);
                        p.AddWithValue("@type", type);
                        p.AddWithValue("@start", start);
                        p.AddWithValue("@end", end);
                        p.AddWithValue("@createdBy", _loggedInUserId);
                        p.AddWithValue("@lastUpdateBy", _loggedInUserId);
                        p.AddWithValue("@title", "not needed");
                        p.AddWithValue("@description", "not needed");
                        p.AddWithValue("@location", "not needed");
                        p.AddWithValue("@contact", "not needed");
                        p.AddWithValue("@url", "not needed");
                        
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            MessageBox.Show($"Error adding appointment", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        MessageBox.Show($"Appointment added successfully.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                await GetAppointmentsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding appointment: {ex.Message}");
            }
        }

        private async void btnUpdateAppointment_Click(object sender, EventArgs e)
        {
            if (dataGridViewAppointments.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an appointment to update.");
                return;
            }

            int appointmentId = Convert.ToInt32(dataGridViewAppointments.SelectedRows[0].Cells["appointmentId"].Value);
            string type = txtType.Text.Trim();
            DateTime start = ConvertToUtcTime(dateTimePickerStart.Value);
            DateTime end = ConvertToUtcTime(dateTimePickerEnd.Value);
            int customerId = (int)comboBoxCustomer.SelectedValue;

            if (string.IsNullOrEmpty(type) || start >= end || comboBoxCustomer.SelectedValue == null)
            {
                MessageBox.Show("Please fill in all fields correctly.");
                return;
            }

            try
            {
                if (!IsInBusinessHours(start, end))
                {
                    MessageBox.Show("Appointments must be scheduled between 9:00 AM and 5:00 PM, Monday to Friday (EST).");
                    return;
                }

                if (await DoAppointmentsOverlap(customerId, start, end))
                {
                    MessageBox.Show("The appointment overlaps with an existing appointment. Please choose another time.");
                    return;
                }

                using (var conn = _dbConFact.CreateConnection())
                {
                    await conn.OpenAsync();

                    string query =
                        "UPDATE appointment SET customerId = @customerId, type = @type, start = @start, end = @end, lastUpdate = NOW(), lastUpdateBy = @lastUpdateBy WHERE appointmentId = @appointmentId;";

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        var p = cmd.Parameters;

                        p.AddWithValue("@customerId", customerId);
                        p.AddWithValue("@type", type);
                        p.AddWithValue("@start", start);
                        p.AddWithValue("@end", end);
                        p.AddWithValue("@lastUpdateBy", _loggedInUserId);
                        p.AddWithValue("@appointmentId", appointmentId);

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            MessageBox.Show($"Error updating appointment", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        MessageBox.Show($"Appointment updated successfully.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                await GetAppointmentsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating appointment: {ex.Message}");
            }
        }

        private async void btnDeleteAppointment_Click(object sender, EventArgs e)
        {
            if (dataGridViewAppointments.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an appointment to delete.");
                return;
            }

            int appointmentId = Convert.ToInt32(dataGridViewAppointments.SelectedRows[0].Cells["appointmentId"].Value);

            DialogResult dialogResult = MessageBox.Show("Are you sure you want to delete this appointment?",
                "Confirm Deletion", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.No)
            {
                return;
            }

            try
            {
                using (var conn = _dbConFact.CreateConnection())
                {

                    string query = "DELETE FROM appointment WHERE appointmentId = @appointmentId;";
                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@appointmentId", appointmentId);
                        
                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected == 0)
                        {
                            MessageBox.Show($"Error deleting appointment", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        MessageBox.Show($"Appointment deleted successfully.", "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }

                await GetAppointmentsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting appointment: {ex.Message}");
            }
        }

        private void dataGridViewAppointments_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridViewAppointments.SelectedRows.Count > 0)
            {
                DataGridViewRow selectedRow = dataGridViewAppointments.SelectedRows[0];
                txtType.Text = selectedRow.Cells["type"].Value.ToString();
                dateTimePickerStart.Value = Convert.ToDateTime(selectedRow.Cells["start"].Value);
                dateTimePickerEnd.Value = Convert.ToDateTime(selectedRow.Cells["end"].Value);
                comboBoxCustomer.SelectedValue = Convert.ToInt32(selectedRow.Cells["customerId"].Value);
            }
        }
        private void btnClearFields_Click(object sender, EventArgs e)
        {
            txtType.Clear();
            dateTimePickerStart.ResetText();
            dateTimePickerEnd.ResetText();
            comboBoxCustomer.SelectedIndex = 0;
        }

        #endregion


        #region Helper Methods
        private bool IsInBusinessHours(DateTime start, DateTime end)
        {
            TimeZoneInfo est = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            DateTime estStart = TimeZoneInfo.ConvertTime(start, est);
            DateTime estEnd = TimeZoneInfo.ConvertTime(end, est);

            if (estStart.TimeOfDay < new TimeSpan(9, 0, 0) || estEnd.TimeOfDay > new TimeSpan(17, 0, 0))
            {
                return false;
            }

            if (estStart.DayOfWeek == DayOfWeek.Saturday || estStart.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            return true;
        }

        private async Task<bool> DoAppointmentsOverlap(int customerId, DateTime start, DateTime end, int? appointmentId = null)
        {
            bool isExistingAppt = false;

            if (appointmentId.HasValue)
            {
                isExistingAppt = true;
            }

            try
            {
                using (var conn = _dbConFact.CreateConnection())
                {
                    await conn.OpenAsync();

                    string query = "";

                    if (isExistingAppt)
                    {
                        query = query = @"SELECT COUNT(*) FROM appointment WHERE customerId = @customerId AND (@start < end AND @end > start) AND appointmentId != @appointmentId";
                    }
                    else
                    {
                        query = @"SELECT COUNT(*) FROM appointment WHERE customerId = @customerId AND (@start < end AND @end > start);";
                    }

                    using (MySqlCommand cmd = new MySqlCommand(query, conn))
                    {
                        var p = cmd.Parameters;

                        if (!isExistingAppt)
                        {
                            p.AddWithValue("@customerId", customerId);
                            p.AddWithValue("@start", start);
                            p.AddWithValue("@end", end);
                        }
                        else
                        {
                            p.AddWithValue("@customerId", customerId);
                            p.AddWithValue("@start", start);
                            p.AddWithValue("@end", end);
                            p.AddWithValue("@appointmentId", appointmentId.Value);
                        }

                        int rowsAffected = Convert.ToInt32(cmd.ExecuteScalar());

                        return rowsAffected > 0;


                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error checking overlapping appointments: {ex.Message}");
                return true;
            }
        }

        private DateTime ConvertToUtcTime(DateTime localTime)
        {
            return TimeZoneInfo.ConvertTimeToUtc(localTime);
        }

        private DateTime ConvertToLocalTime(DateTime utcTime)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, TimeZoneInfo.Local);
        }

        #endregion

        
    }
}
