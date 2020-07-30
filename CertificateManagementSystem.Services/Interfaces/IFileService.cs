using CertificateManagementSystem.Data.Models;
using Microsoft.AspNetCore.Http;

namespace CertificateManagementSystem.Services.Interfaces
{
    public interface IFileService
    {
        bool UploadFile(IFormFile file, string destinationPath);
        string ActualizeFilePath(FileModel file);
    }
}
