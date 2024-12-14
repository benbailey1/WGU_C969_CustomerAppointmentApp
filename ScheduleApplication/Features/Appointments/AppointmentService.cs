using ScheduleApplication.Features.Appointments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScheduleApplication.Features.Appointments
{
    public interface IAppointmentService
    {
        Task<AppointmentResult<int>> CreateAppointmentAsync(AppointmentModel appt);
        Task<AppointmentResult<List<AppointmentModel>>> GetAllAppointmentsAsync();
        Task<AppointmentResult<bool>> UpdateAppointmentAsync(AppointmentModel appt);
        Task<AppointmentResult<bool>> DeleteAppointmentAsync(int id);
        Task<AppointmentResult<List<MonthlyAppointmentTypes>>> GetAppointmentTypesByMonthAsync();
        Task<AppointmentResult<Dictionary<string, List<AppointmentModel>>>> GetScheduleForEachUserAsync();
        Task<AppointmentResult<List<LocationAppointmentSummary>>> GetAppointmentsByLocationAsync();
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

        public async Task<AppointmentResult<List<MonthlyAppointmentTypes>>> GetAppointmentTypesByMonthAsync()
        {
            try
            {
                var appointmentsRes = await GetAllAppointmentsAsync();

                if (!appointmentsRes.IsSuccess)
                {
                    return AppointmentResult<List<MonthlyAppointmentTypes>>.Failure(appointmentsRes.Errors.First());
                }

                var monthlyTypes = appointmentsRes.Value
                    .GroupBy(a => a.Start.ToString("MMMM"))
                    .Select(monthGroup => new MonthlyAppointmentTypes
                    {
                        Month = monthGroup.Key,
                        TypeCounts = monthGroup
                            .GroupBy(a => a.Type)
                            .Select(typeGroup => new TypeCount
                            {
                                Type = typeGroup.Key,
                                Count = typeGroup.Count()
                            })
                            .ToList()
                    })
                    .ToList();

                return AppointmentResult<List<MonthlyAppointmentTypes>>.Success(monthlyTypes);
            }
            catch (Exception ex)
            {
                return AppointmentResult<List<MonthlyAppointmentTypes>>.Failure($"Error getting appointment types: {ex.Message}");
            }
        }

        public async Task<AppointmentResult<Dictionary<string, List<AppointmentModel>>>> GetScheduleForEachUserAsync()
        {
            try
            {
                var appointmentsRes = await GetAllAppointmentsAsync();

                if (!appointmentsRes.IsSuccess)
                {
                    return AppointmentResult<Dictionary<string, List<AppointmentModel>>>.Failure(appointmentsRes.Errors.First());
                }

                var userSched = appointmentsRes.Value 
                                .GroupBy(a => a.UserId)
                                .ToDictionary(g => g.Key.ToString(), g => g.ToList());

                return AppointmentResult<Dictionary<string, List<AppointmentModel>>>.Success(userSched);
            }
            catch (Exception ex)
            {
                return AppointmentResult<Dictionary<string, List<AppointmentModel>>>.Failure($"Error getting appointments by user. {ex.Message}"); ;
            }
        }

        public async Task<AppointmentResult<List<LocationAppointmentSummary>>> GetAppointmentsByLocationAsync()
        {
            try
            {
                var appointmentsRes = await GetAllAppointmentsAsync();

                if (!appointmentsRes.IsSuccess)
                {
                    return AppointmentResult<List<LocationAppointmentSummary>>.Failure(appointmentsRes.Errors.First());
                }

                var locationSummaries = appointmentsRes.Value
                    .GroupBy(a => a.Location ?? "Unspecified")
                    .Select(group => new LocationAppointmentSummary
                    {
                        Location = group.Key,
                        TotalAppointments = group.Count(),
                        UniqueCustomers = group.Select(a => a.CustomerId).Distinct().Count(),
                        AppointmentTypes = group.GroupBy(a => a.Type)
                                            .Select(t => new TypeSummary
                                            {
                                                Type = t.Key,
                                                Count = t.Count()
                                            })
                                            .OrderByDescending(t => t.Count)
                                            .ToList(),
                        UpcomingAppointments = group.Where(a => a.Start > DateTime.Now)
                                                .OrderBy(a => a.Start)
                                                .ToList()
                    })
                    .OrderByDescending(l => l.TotalAppointments)
                    .ToList();

                return AppointmentResult<List<LocationAppointmentSummary>>.Success(locationSummaries);
            }
            catch (Exception ex)
            {
                return AppointmentResult<List<LocationAppointmentSummary>>.Failure(
                    $"Error getting appointments by location: {ex.Message}");
            }
        }
    }
}
