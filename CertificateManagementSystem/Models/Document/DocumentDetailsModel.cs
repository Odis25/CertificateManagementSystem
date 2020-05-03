using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Models.Document
{
    public class DocumentDetailsModel
    {
        public int Year { get; set; }

        public string ContractNumber { get; set; }

        public string RegistrationNumber { get; set; }
        public string VerificationMethodic { get; set; }

        public string ClientName { get; set; }
        public string ExploitationPlace { get; set; }

        public string DeviceType { get; set; }
        public string DeviceName { get; set; }

        public string DocumentNumber { get; set; }
        public string DocumentType { get; set; }
        public string SerialNumber { get; set; }

        public string DocumentDate { get; set; }
        public string CalibrationDate { get; set; }
        public string CalibrationExpireDate { get; set; }

        public string FilePath { get; set; }
    }
}
