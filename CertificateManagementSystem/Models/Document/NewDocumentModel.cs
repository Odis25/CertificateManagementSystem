using System;

namespace CertificateManagementSystem.Models.Document
{
    public class NewDocumentModel
    {
        public int Year { get; set; }

        public string ContractNumber { get; set; }
        public string ClientName { get; set; }
        public string ObjectName { get; set; }

        public string RegistrationNumber { get; set; }
        public string FilePath { get; set; }

        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string SerialNumber { get; set; }

        public DateTime CalibrationDate { get; set; }
        public DateTime CalibrationExpireDate { get; set; }
    }
}
