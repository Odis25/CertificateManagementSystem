using System;

namespace CertificateManagementSystem.Data.Models
{
    public class Certificate: Document
    {
        public DateTime CalibrationDate { get; set; }
        public DateTime CalibrationExpireDate { get; set; }        
    }
}
