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
using ScheduleApplication.Shared.Infrastructure.Database;

namespace ScheduleApplication.Features.Main
{
    public partial class MainForm : Form
    {
        private int _loggedInUserId;
        private IAppointmentService _apptService;
        public MainForm(int loggedInUserId, IDbConnectionFactory dbConFact)
        {
            InitializeComponent();
            _loggedInUserId = loggedInUserId;

            var appointmentRepo = new AppointmentRepository(dbConFact);
            var appointmentValidator = new AppointmentValidator();
            _apptService = new AppointmentService(appointmentRepo, appointmentValidator);
        }

        private void btnManageAppointments_Click(object sender, EventArgs e)
        {
            AppointmentManagementForm appointmentForm = new AppointmentManagementForm(_apptService);
            appointmentForm.Show();
        }

    }
}
