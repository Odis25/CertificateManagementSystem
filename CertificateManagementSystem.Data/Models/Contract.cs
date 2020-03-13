
namespace CertificateManagementSystem.Data.Models
{
    public class Contract
    {
        public int Id { get; set; }

        public int Year { get; set; }
        public string ContractNumber { get; set; }

        public virtual Client Client { get; set; }
    }
}
