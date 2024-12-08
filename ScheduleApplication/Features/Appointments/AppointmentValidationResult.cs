using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleApplication.Features.Appointments
{
    public class AppointmentValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; }

        public AppointmentValidationResult() 
        {
            IsValid = true;
            Errors = new List<string>();
        }

        public void AddError(string error)
        {
            IsValid = false;
            Errors.Add(error);
        }
    }
}
