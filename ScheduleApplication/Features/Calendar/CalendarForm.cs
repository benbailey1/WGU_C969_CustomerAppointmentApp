using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScheduleApplication.Features.Appointments;

namespace ScheduleApplication.Features.Calendar
{
    public partial class CalendarForm : Form
    {
        private readonly IAppointmentRepository _apptRepo;
        public CalendarForm(IAppointmentRepository apptRepo)
        {
            InitializeComponent();
            _apptRepo = apptRepo;
        }

        private async void AppointmentCalendar_DateChanged(object sender, DateRangeEventArgs e)
        {
            DateTime selected = e.Start;

            var res = await _apptRepo.GetAllAppointmentsAsync();
            if (res.IsSuccess)
            {
                var appointments = res.Value
                    .Where(a => a.Start.Date == selected.Date)
                    .ToList();

                dataGridViewAppointments.DataSource = appointments;
            }    
            else
            {
                MessageBox.Show("No appointments found for selected date");
            }
                
        }
    }
}
