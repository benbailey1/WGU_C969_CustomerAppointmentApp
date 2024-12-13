using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
