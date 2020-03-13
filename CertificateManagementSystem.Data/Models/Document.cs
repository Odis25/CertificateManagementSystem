namespace CertificateManagementSystem.Data.Models
{
    public abstract class Document
    {
        public int Id { get; set; }

        public string DocumentNumber { get; set; }
        public string FilePath { get; set; }
        
        public virtual Device Device { get; set; }
    }
}
