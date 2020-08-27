using System.Collections.Generic;

namespace CertificateManagementSystem.Services.Models
{
    public class YearDTO
    {
        public int Year { get; set; }
        public IEnumerable<ContractDTO> Contracts { get; set; }
    }
}
