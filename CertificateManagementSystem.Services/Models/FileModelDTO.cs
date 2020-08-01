namespace CertificateManagementSystem.Services.Models
{
    public class FileModelDTO
    {
        public int Id { get; set; }
        public long Size { get; set; }
        public string Path { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
