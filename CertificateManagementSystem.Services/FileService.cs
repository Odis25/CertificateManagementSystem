using CertificateManagementSystem.Data;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace CertificateManagementSystem.Services
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;

        public FileService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Создать файл документа
        public void CreateFile(string sourcePath, string destinationPath)
        {
            var documentsFolderPath = _configuration.GetSection("Paths").GetSection("DocumentFolder").Value;
            destinationPath = Path.Combine(documentsFolderPath, destinationPath);

            var folder = Path.GetDirectoryName(destinationPath); 
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            File.Copy(sourcePath, destinationPath);
        }
    }
}
