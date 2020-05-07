using System;

namespace CertificateManagementSystem.Data.Models
{
    public abstract class Document
    {
        public int Id { get; set; }

        public string DocumentNumber { get; set; }
        
        public Client Client { get; set; }
        public Contract Contract { get; set; }
        public Device Device { get; set; }
        public FileModel DocumentFile { get; set; }

        public DateTime CreatedOn { get; set; } 
        public DateTime? UpdatedOn { get; set; } 

        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
    }
}
