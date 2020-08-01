using System.Collections.Generic;

namespace CertificateManagementSystem.Services.Models
{
    public class ContractDTO
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string ContractNumber { get; set; }
        public IEnumerable<DocumentDTO> Documents { get; set; }
    }
}
