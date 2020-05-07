using CertificateManagementSystem.Data.Models;

namespace CertificateManagementSystem.Data
{
    public interface IFileService
    {
        void CreateFile(string sourceFilePath, string destinationFilePath);
        string GetRealFilePath(FileModel file);
    }
}
