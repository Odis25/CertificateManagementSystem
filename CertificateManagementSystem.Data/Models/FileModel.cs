namespace CertificateManagementSystem.Data.Models
{
    public class FileModel
    {
        public int Id { get; set; }

        public long Size { get; set; }

        public string Path { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
    }
}
