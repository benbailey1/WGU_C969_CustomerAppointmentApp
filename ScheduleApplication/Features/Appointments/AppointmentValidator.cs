using ScheduleApplication.Features.Appointments.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScheduleApplication.Features.Appointments
{
    public interface IAppointmentValidator
    {
        AppointmentResult<AppointmentModel> Validate(AppointmentModel appointment, IEnumerable<AppointmentModel> existingAppointments);
    }
    public class AppointmentValidator : IAppointmentValidator
    {
        private static readonly TimeZoneInfo EstTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        public AppointmentResult<AppointmentModel> Validate(AppointmentModel appointment, IEnumerable<AppointmentModel> existingAppointments)
        {
            var errors = new List<string>();

            if (appointment == null)
                return AppointmentResult<AppointmentModel>.Failure("Appointment cannot be null");

            ValidateRequiredFields(appointment, errors);
            validateBusinessHours(appointment, errors);
            ValidateAppointmentDuration(appointment, errors);
            ValidateOverlappingAppointments(appointment, existingAppointments, errors);

            if (errors.Any())
                return AppointmentResult<AppointmentModel>.ValidationError(errors);

            return AppointmentResult<AppointmentModel>.Success(appointment);
        }

        private void ValidateRequiredFields(AppointmentModel appointment, List<string> errors)
        {

            if (appointment.CustomerId <= 0)
                errors.Add("Customer ID is required.");

            if (appointment.UserId <= 0)
                errors.Add("User ID is required.");

            if (string.IsNullOrWhiteSpace(appointment.Title))
                errors.Add("Appointment Title is required.");

            if (string.IsNullOrWhiteSpace(appointment.Type))
                errors.Add("Appointment Type is required.");

            if (appointment.Start == DateTime.MinValue)
                errors.Add("End time is required.");

            if (appointment.End <= appointment.Start)
                errors.Add("End time must be after start time.");
        }

        private void validateBusinessHours(AppointmentModel appointment, List<string> errors)
        {
            // Convert to EST
            var startEst = TimeZoneInfo.ConvertTimeFromUtc(appointment.Start.ToUniversalTime(), EstTimeZone);
            var endEst = TimeZoneInfo.ConvertTimeFromUtc(appointment.End.ToUniversalTime(), EstTimeZone);

            // Verify appt isn't on weekend
            if (startEst.DayOfWeek == DayOfWeek.Saturday || startEst.DayOfWeek == DayOfWeek.Sunday ||
                endEst.DayOfWeek == DayOfWeek.Saturday || endEst.DayOfWeek == DayOfWeek.Sunday)
            {
                errors.Add("Appointments can only be scheduled Monday through Friday.");
                return;
            }

            // Verify appt w/in business hrs (9AM - 5PM EST)
            var businessStart = new TimeSpan(9, 0, 0);
            var businessEnd = new TimeSpan(17, 0, 0);

            if (startEst.TimeOfDay < businessStart || endEst.TimeOfDay > businessEnd)
            {
                errors.Add("Appointments must be scheduled between 9:00 AM and 5:00 PM EST.");
            }
        }

        private void ValidateOverlappingAppointments(AppointmentModel appointment, 
            IEnumerable<AppointmentModel> existingAppointments, List<string> errors)
        {
            var overlappingAppointments = existingAppointments.Where(existing =>
                existing.AppointmentId != appointment.AppointmentId &&
                existing.UserId == appointment.UserId &&
                !(existing.End <= appointment.Start || existing.Start >= appointment.End))
                .ToList();

            if (overlappingAppointments.Any())
            {
                errors.Add("This appointment overlaps with existing appointments.");
                // TODO: NOT MVP - return suggested available appointment times that don't overlap 
            }
        }

        private void ValidateAppointmentDuration(AppointmentModel appointment, List<string> errors)
        {
            var apptDuration = appointment.End - appointment.Start;

            if (apptDuration.TotalMinutes < 15)
                errors.Add("Appointment must be at least 15 mins long.");

            if (apptDuration.TotalHours > 1)
                errors.Add("Appointment cannot be longer that 1 hour.");
        }


    }
}
