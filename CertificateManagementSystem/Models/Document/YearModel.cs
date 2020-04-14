using System.Collections.Generic;

namespace CertificateManagementSystem.Models.Document
{
    public class YearModel
    {
        public int Year { get; set; }
        public IEnumerable<ContractModel> Contracts { get; set; }
    }
}
