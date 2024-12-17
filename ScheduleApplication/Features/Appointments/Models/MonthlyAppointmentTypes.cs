using System.Collections.Generic;


namespace ScheduleApplication.Features.Appointments
{
    public class MonthlyAppointmentTypes
    {
        public string Month { get; set; }
        public List<TypeCount> TypeCounts { get; set; }

        public MonthlyAppointmentTypes()
        {
            TypeCounts = new List<TypeCount>();
        }
    }

    public class TypeCount
    {
        public string Type { get; set; }
        public int Count { get; set; }
    }
}
