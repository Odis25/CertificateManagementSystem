namespace CertificateManagementSystem.Data.Models
{
    public abstract class Document
    {
        public int Id { get; set; }

        public string DocumentNumber { get; set; }
        
        public virtual FileModel DocumentFile { get; set; }      
        public virtual Device Device { get; set; }
        public virtual Contract Contract { get; set; }
    }
}
