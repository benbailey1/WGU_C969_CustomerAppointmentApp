using ScheduleApplication.Features.Appointments.Models;
using System.Collections.Generic;
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
        private readonly IAppointmentValidator _apptValidate;

        public AppointmentService(IAppointmentRepository apptRepo, IAppointmentValidator apptValidate) 
        {
            _apptRepo = apptRepo;
            _apptValidate = apptValidate;
        }

        public async Task<AppointmentResult<int>> CreateAppointmentAsync(AppointmentModel appt)
        {
            // Get existing appointments to check against new appointment
            var existingAppts = await _apptRepo.GetAllAppointmentsAsync();
            if (!existingAppts.IsSuccess)
            {
                return AppointmentResult<int>.Failure(existingAppts.Errors);
            }

            // validate new appointment against existing 
            var validateRes = _apptValidate.Validate(appt, existingAppts.Value);
            if (!validateRes.IsSuccess)
            {
                return AppointmentResult<int>.ValidationError(validateRes.Errors);
            }

            var result = await _apptRepo.AddAppointmentAsync(appt);
            if (!result.IsSuccess)
            {
                return result;
            }

            return AppointmentResult<int>.Success(result.Value);

        }

        public async Task<AppointmentResult<List<AppointmentModel>>> GetAllAppointmentsAsync()
        {
            return await _apptRepo.GetAllAppointmentsAsync();
        }

        public async Task<AppointmentResult<bool>> UpdateAppointmentAsync(AppointmentModel appt)
        {
            // Get existing appointments to check against new appointment
            var existingAppts = await _apptRepo.GetAllAppointmentsAsync();
            if (!existingAppts.IsSuccess)
            {
                return AppointmentResult<bool>.Failure(existingAppts.Errors);
            }

            // validate new appointment against existing 
            var validateRes = _apptValidate.Validate(appt, existingAppts.Value);
            if (!validateRes.IsSuccess)
            {
                return AppointmentResult<bool>.ValidationError(validateRes.Errors);
            }

            var result = await _apptRepo.UpdateAppointmentAsync(appt);
            if (!result.IsSuccess)
            {
                return result;
            }

            return AppointmentResult<bool>.Success(true);
        }
        
        public async Task<AppointmentResult<bool>> DeleteAppointmentAsync(int id)
        {
            var result = await _apptRepo.DeleteAppointmentAsync(id);
            if (!result.IsSuccess)
            {
                return result;
            }

            return AppointmentResult<bool>.Success(true);
        }
    }
}
