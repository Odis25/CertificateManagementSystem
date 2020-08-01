using CertificateManagementSystem.Services.Interfaces;
using CertificateManagementSystem.Services.Models;
using Microsoft.AspNetCore.Http;
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
        public bool UploadFile(IFormFile file, string destinationPath)
        {
            var documentsFolderPath = _configuration.GetSection("Paths").GetSection("DocumentsFolder").Value;
            destinationPath = Path.Combine(documentsFolderPath, destinationPath);
            var destinationFolder = Path.GetDirectoryName(destinationPath);
            
            // Директория не существует?
            if (!Directory.Exists(destinationFolder))
                Directory.CreateDirectory(destinationFolder);

            if (File.Exists(destinationPath))
                return false;

            using (var document = new FileStream(destinationPath, FileMode.Create))
            {
                file.CopyTo(document);
            }
            return true;
        }

        public string ActualizeFilePath(FileModelDTO file)
        {
            var documentsFolderPath = _configuration.GetSection("Paths").GetSection("DocumentsFolder").Value;
            var filePath = Path.Combine(documentsFolderPath, file.Path);
            var fileFolder = Path.GetDirectoryName(filePath);
            var fileName = Path.GetFileNameWithoutExtension(filePath);
            var extension = Path.GetExtension(filePath);
            var i = 0;

            // Проверить существет ли файл с таким именем
            while (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath);

                if (file.Size == fileInfo.Length)
                    return filePath.Replace(documentsFolderPath, "");

                var newFileName = $"{fileName}_({i++})";
                filePath = Path.Combine(fileFolder, newFileName + extension);
            }

            return filePath.Replace(documentsFolderPath, "");
        }
    }
}
