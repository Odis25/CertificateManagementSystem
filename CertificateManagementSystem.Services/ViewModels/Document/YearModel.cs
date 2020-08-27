using System.Collections.Generic;

namespace CertificateManagementSystem.Services.ViewModels.Document
{
    public class YearModel
    {
        public int Year { get; set; }
        public IEnumerable<ContractModel> Contracts { get; set; }
    }
}
