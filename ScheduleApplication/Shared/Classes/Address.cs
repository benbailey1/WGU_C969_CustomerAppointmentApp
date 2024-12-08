using ScheduleApplication.Shared.Domain;

namespace ScheduleApplication.Shared.Classes
{
    public class Address
    {
        public int AddressId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public int CityId { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }
        public AuditInfo AuditInfo { get; set; }
    }
}
