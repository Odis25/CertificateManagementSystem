using System.Collections.Generic;

namespace CertificateManagementSystem.Services.Models
{
    public class DeviceDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string SerialNumber { get; set; }
        public string RegistrationNumber { get; set; }
        public MethodicDTO VerificationMethodic { get; set; }
        public IEnumerable<DocumentDTO> Documents { get; set; }
    }
}
