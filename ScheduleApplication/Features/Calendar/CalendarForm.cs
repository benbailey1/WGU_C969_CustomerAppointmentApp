using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScheduleApplication.Features.Appointments;

namespace ScheduleApplication.Features.Calendar
{
    public partial class CalendarForm : Form
    {
        private readonly IAppointmentRepository _apptRepo;

        private DateTime selectedStart;
        private DateTime selectedEnd;
        public CalendarForm(IAppointmentRepository apptRepo)
        {
            InitializeComponent();
            _apptRepo = apptRepo;
            InitializeSelectionControls();
        }

        private void InitializeSelectionControls()
        {
            AppointmentCalendar.MaxSelectionCount = 1; // Start with single day selection

            // Add event handlers
            rbSelectDay.CheckedChanged += SelectionMode_Changed;
            rbSelectMonth.CheckedChanged += SelectionMode_Changed;

            this.Controls.AddRange(new Control[] { rbSelectDay, rbSelectMonth });
        }

        private async void SelectionMode_Changed(object sender, EventArgs e)
        {
            if (rbSelectMonth.Checked)
            {
                // When "Select Month" is chosen, auto-select the entire month
                DateTime firstDay = new DateTime(AppointmentCalendar.SelectionStart.Year,
                                              AppointmentCalendar.SelectionStart.Month, 1);
                DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);
                AppointmentCalendar.MaxSelectionCount = 31; // Allow multi-day selection
                AppointmentCalendar.SelectionRange = new SelectionRange(firstDay, lastDay);

                // Update the grid with the month's appointments
                await LoadAppointments(firstDay, lastDay);
            }
            else
            {
                // When "Select Day" is chosen, only allow single day selection
                AppointmentCalendar.MaxSelectionCount = 1;
                await LoadAppointments(AppointmentCalendar.SelectionStart, AppointmentCalendar.SelectionStart);
            }
        }

        private async void AppointmentCalendar_DateChanged(object sender, DateRangeEventArgs e)
        {
            if (rbSelectMonth.Checked)
            {
                // When month selection is active, always select the entire month
                DateTime firstDay = new DateTime(e.Start.Year, e.Start.Month, 1);
                DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);
                AppointmentCalendar.SelectionRange = new SelectionRange(firstDay, lastDay);
                selectedStart = firstDay;
                selectedEnd = lastDay;

                await LoadAppointments(firstDay, lastDay);
            }
            else
            {
                // For day selection, just use the single selected date
                selectedStart = selectedEnd = e.Start;
                await LoadAppointments(e.Start, e.Start);
            }
        }

        private async Task LoadAppointments(DateTime start, DateTime end)
        {
            var res = await _apptRepo.GetAllAppointmentsAsync();
            if (res.IsSuccess)
            {
                var appointments = res.Value
                    .Where(a => a.Start.Date >= start.Date && a.Start.Date <= end.Date)
                    .ToList();

                if (appointments.Any())
                {
                    dataGridViewAppointments.DataSource = appointments;
                }
                else
                {
                    dataGridViewAppointments.DataSource = null;
                }
            }
            else
            {
                MessageBox.Show("Error loading appointments");
                dataGridViewAppointments.DataSource = null;
            }
        }
    }
}