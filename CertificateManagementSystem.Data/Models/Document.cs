namespace CertificateManagementSystem.Data.Models
{
    public abstract class Document
    {
        public int Id { get; set; }

        public string RegistrationNumber { get; set; }
        public string FilePath { get; set; }
        
        public virtual Device Device { get; set; }
        public virtual VerificationMethodic VerificationMethodic { get; set; }
    }
}
