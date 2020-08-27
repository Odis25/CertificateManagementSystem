namespace CertificateManagementSystem.Services.ViewModels.Document
{
    public class DocumentListingModel
    {
        public int Id { get; set; }
        public int Year { get; set; }

        public int ContractId { get; set; }
        public string ContractNumber { get; set; }

        public string RegistrationNumber { get; set; }
        public string VerificationMethodic { get; set; }

        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public string ExploitationPlace { get; set; }

        public int DeviceId { get; set; }
        public string DeviceType { get; set; }
        public string DeviceName { get; set; }

        public string DocumentNumber { get; set; }
        public string DocumentType { get; set; }
        public string SerialNumber { get; set; }

        public string DocumentDate { get; set; }
        public string CalibrationDate { get; set; }
        public string CalibrationExpireDate { get; set; }

        public string CreatedOn { get; set; }
        public string UpdatedOn { get; set; }

        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public string FilePath { get; set; }
    }
}
