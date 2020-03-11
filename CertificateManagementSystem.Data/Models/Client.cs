using System.Collections.Generic;

namespace CertificateManagementSystem.Data.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }

        // Название объектов эксплуатации
        public IEnumerable<ExploitationPlace> ExploitationPlaces { get; set; }
    }
}
