using System.Collections.Generic;

namespace CertificateManagementSystem.Services.Models
{
    public class ClientDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ExploitationPlace { get; set; }
        public IEnumerable<DocumentDTO> Documents { get; set; }
    }
}
