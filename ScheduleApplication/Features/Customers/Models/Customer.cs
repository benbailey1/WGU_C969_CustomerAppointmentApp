using ScheduleApplication.Shared.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduleApplication.Features.Customers
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Address Address { get; set; }
        public bool Active { get; set; }
        public AuditInfo AuditInfo { get; set; }
    }
}
