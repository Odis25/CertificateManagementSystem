using CertificateManagementSystem.Data.Models;

namespace CertificateManagementSystem.Services.Interfaces
{
    public interface IFileService
    {
        void CreateFile(string sourceFilePath, string destinationFilePath);
        string GetRealFilePath(FileModel file);
    }
}
