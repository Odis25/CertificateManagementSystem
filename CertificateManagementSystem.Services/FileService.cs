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
        public void CreateFile(string sourceFilePath, ref string destinationFilePath)
        {
            var documentsFolderPath = _configuration.GetSection("Paths").GetSection("DocumentsFolder").Value;
            destinationFilePath = Path.Combine(documentsFolderPath, destinationFilePath);

            var folder = Path.GetDirectoryName(destinationFilePath);

            // Директория не существует?
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            
            File.Copy(sourceFilePath, destinationFilePath);
        }

    }
}
