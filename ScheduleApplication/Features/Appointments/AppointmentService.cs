using ScheduleApplication.Features.Appointments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleApplication.Features.Appointments
{
    public interface IAppointmentService
    {
        Task<AppointmentResult<int>> CreateAppointmentAsync(AppointmentModel appt);
        Task<AppointmentResult<List<AppointmentModel>>> GetAllAppointmentsAsync();
        Task<AppointmentResult<bool>> UpdateAppointmentAsync(AppointmentModel appt);
        Task<AppointmentResult<bool>> DeleteAppointmentAsync(int id);
    }
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _apptRepo;

        public AppointmentService() { }

        public Task<AppointmentResult<int>> CreateAppointmentAsync(AppointmentModel appt)
        {
            throw new NotImplementedException();
        }

        public Task<AppointmentResult<List<AppointmentModel>>> GetAllAppointmentsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<AppointmentResult<bool>> UpdateAppointmentAsync(AppointmentModel appt)
        {
            throw new NotImplementedException();
        }
        
        public Task<AppointmentResult<bool>> DeleteAppointmentAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
