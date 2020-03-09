namespace CertificateManagementSystem.Data.Models
{
    public abstract class Document
    {
        public int Id { get; set; }

        public string RegistrationNumber { get; set; }
        public string FilePath { get; set; }
        
        public VerificationMethodic VerificationMethodic { get; set; }
        public Contract Contract { get; set; }
        public Device Device { get; set; }        
    }
}
