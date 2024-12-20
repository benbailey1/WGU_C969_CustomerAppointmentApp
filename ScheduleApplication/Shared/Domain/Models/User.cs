namespace ScheduleApplication.Shared.Classes
{
    public class User
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public Address Address { get; set; }
        public bool Active { get; set; }
        public AuditInfo AuditInfo { get; set; }
    }
}
