using System.Collections.Generic;

namespace CertificateManagementSystem.Data.Models
{
    public class Client
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string ExploitationPlace { get; set; }

        public IEnumerable<Document> Documents { get; set; }
    }
}
