using System;

namespace CertificateManagementSystem.Services.Models
{
    public abstract class DocumentDTO
    {
        public int Id { get; set; }
        public string DocumentNumber { get; set; }
        public ClientDTO Client { get; set; }
        public ContractDTO Contract { get; set; }
        public DeviceDTO Device { get; set; }
        public FileModelDTO DocumentFile { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
