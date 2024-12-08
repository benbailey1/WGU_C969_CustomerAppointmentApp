using Google.Protobuf.WellKnownTypes;
using ScheduleApplication.Features.Appointments.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleApplication.Features.Appointments
{
    public interface IAppointmentValidator
    {
        AppointmentValidationResult Validate(AppointmentModel appointment, IEnumerable<AppointmentModel> existingAppointments);
    }
    public class AppointmentValidator : IAppointmentValidator
    {
        private static readonly TimeZoneInfo EstTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public AppointmentValidationResult Validate(AppointmentModel appointment, IEnumerable<AppointmentModel> existingAppointments)
        {
            var result = new AppointmentValidationResult();

            ValidateRequiredFields(appointment, result);
            validateBusinessHours(appointment, result);
            ValidateAppointmentDuration(appointment, result);
            ValidateOverlappingAppointments(appointment, existingAppointments, result);

            return result;
        }

        private void ValidateRequiredFields(AppointmentModel appointment, AppointmentValidationResult result)
        {
            if (appointment == null)
            {
                result.AddError("Appointment cannot be null.");
                return;
            }

            if (appointment.CustomerId <= 0)
                result.AddError("Customer ID is required.");

            if (appointment.UserId <= 0)
                result.AddError("User ID is required.");

            if (string.IsNullOrWhiteSpace(appointment.Title))
                result.AddError("Appointment Title is required.");

            if (string.IsNullOrWhiteSpace(appointment.Type))
                result.AddError("Appointment Type is required.");

            if (appointment.Start == DateTime.MinValue)
                result.AddError("End time is required.");

            if (appointment.End <= appointment.Start)
                result.AddError("End time must be after start time.");
        }

        private void validateBusinessHours(AppointmentModel appointment, AppointmentValidationResult result)
        {
            // Convert to EST
            var startEst = TimeZoneInfo.ConvertTimeFromUtc(appointment.Start.ToUniversalTime(), EstTimeZone);
            var endEst = TimeZoneInfo.ConvertTimeFromUtc(appointment.End.ToUniversalTime(), EstTimeZone);

            // Verify appt isn't on weekend
            if (startEst.DayOfWeek == DayOfWeek.Saturday || startEst.DayOfWeek == DayOfWeek.Sunday ||
                endEst.DayOfWeek == DayOfWeek.Saturday || endEst.DayOfWeek == DayOfWeek.Sunday)
            {
                result.AddError("Appointments can only be scheduled Monday through Friday.");
                return;
            }

            // Verify appt w/in business hrs (9AM - 5PM EST)
            var businessStart = new TimeSpan(9, 0, 0);
            var businessEnd = new TimeSpan(17, 0, 0);

            if (startEst.TimeOfDay < businessStart || endEst.TimeOfDay > businessEnd)
            {
                result.AddError("Appointments must be scheduled between 9:00 AM and 5:00 PM EST.");
            }
        }

        private void ValidateOverlappingAppointments(AppointmentModel appointment, 
            IEnumerable<AppointmentModel> existingAppointments, AppointmentValidationResult result)
        {
            var overlappingAppointments = existingAppointments.Where(existing =>
                existing.AppointmentId != appointment.AppointmentId &&
                existing.UserId == appointment.UserId &&
                !(existing.End <= appointment.Start || existing.Start >= appointment.End))
                .ToList();

            if (overlappingAppointments.Any())
            {
                result.AddError("This appointment overlaps with existing appointments.");
                // TODO: NOT MVP - add a suggested appointment time that doesn't overlap 
            }
        }

        private void ValidateAppointmentDuration(AppointmentModel appointment, AppointmentValidationResult result)
        {
            var apptDuration = appointment.End - appointment.Start;

            if (apptDuration.TotalMinutes < 15)
                result.AddError("Appointment must be at least 15 mins long.");

            if (apptDuration.TotalHours > 1)
                result.AddError("Appointment cannot be longer that 1 hour.");
        }


    }
}
