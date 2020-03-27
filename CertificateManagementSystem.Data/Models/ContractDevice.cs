namespace CertificateManagementSystem.Data.Models
{
    public class ContractDevice
    {
        public int ContractId { get; set; }
        public int DeviceId { get; set; }
        public Contract Contract { get; set; }
        public Device Device { get; set; }
    }
}
