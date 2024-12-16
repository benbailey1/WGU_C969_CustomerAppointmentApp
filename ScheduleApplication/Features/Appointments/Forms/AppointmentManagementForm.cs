using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScheduleApplication.Features.Appointments
{
    public partial class AppointmentManagementForm : Form
    {
        private readonly IAppointmentService _apptService;
        public AppointmentManagementForm(IAppointmentService apptService)
        {
            InitializeComponent();
            _apptService = apptService;
        }
    }
}
