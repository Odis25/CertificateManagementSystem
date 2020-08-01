using System;

namespace CertificateManagementSystem.Services.Models
{
    public class CertificateDTO : DocumentDTO
    {
        public DateTime CalibrationDate { get; set; }
        public DateTime CalibrationExpireDate { get; set; }
    }
}
