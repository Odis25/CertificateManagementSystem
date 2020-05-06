using System.Collections.Generic;

namespace CertificateManagementSystem.Data.Models
{
    public class Device
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string Type { get; set; }
        public string SerialNumber { get; set; }

        public VerificationMethodic VerificationMethodic { get; set; }

        public IEnumerable<Document> Documents { get; set; }
    }
}
