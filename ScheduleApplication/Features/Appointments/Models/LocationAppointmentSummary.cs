using System.Collections.Generic;

namespace ScheduleApplication.Features.Appointments.Models
{
    public class LocationAppointmentSummary
    {
        public string Location { get; set; }
        public int TotalAppointments { get; set; }
        public int UniqueCustomers { get; set; }
        public List<TypeSummary> AppointmentTypes { get; set; }
        public List<AppointmentModel> UpcomingAppointments { get; set; }
    }

    public class TypeSummary
    {
        public string Type { get; set; }
        public int Count { get; set; }
    }
}
