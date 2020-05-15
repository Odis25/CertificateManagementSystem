namespace CertificateManagementSystem.Models.Search
{
    public class SearchModel
    {
        public string SearchQuery { get; set; }

        public bool IsYear { get; set; }
        public bool IsContractNumber { get; set; }
        public bool IsClientName { get; set; }
        public bool IsExploitationPlace { get; set; }
        public bool IsDeviceType { get; set; }
        public bool IsDeviceName { get; set; }
        public bool IsSerialNumber { get; set; }
        public bool IsVerificationMethodic { get; set; }
        public bool IsRegisterNumber { get; set; }
        public bool IsDocumentNumber { get; set; }
        public bool IsDocumentType { get; set; }
        public bool IsCalibrationDate { get; set; }
        public bool IsCalibrationExpireDate { get; set; }
        public bool IsDocumentDate { get; set; }
    }
}
