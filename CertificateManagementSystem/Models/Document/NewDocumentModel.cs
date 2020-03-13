using System;

namespace CertificateManagementSystem.Models.Document
{
    public class NewDocumentModel
    {
        public int Year { get; set; }

        public string ContractNumber { get; set; }
        public string ClientName { get; set; }
        public string DeviceName { get; set; }
        public string DeviceType { get; set; }
        public string DocumentNumber { get; set; }
        public string ExploitationPlace { get; set; }
        public string RegistrationNumber { get; set; }
        public string SerialNumber { get; set; }
        public string VerificationMethodic { get; set; }

        public string FilePath { get; set; }
        
        public DateTime CalibrationDate { get; set; }
        public DateTime CalibrationExpireDate { get; set; }
    }
}
