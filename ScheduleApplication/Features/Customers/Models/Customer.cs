using ScheduleApplication.Shared.Classes;

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
