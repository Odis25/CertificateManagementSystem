using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Services.Interfaces;
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
        public void CreateFile(string sourceFilePath, string destinationFilePath)
        {
            var documentsFolderPath = _configuration.GetSection("Paths").GetSection("DocumentsFolder").Value;
            destinationFilePath = Path.Combine(documentsFolderPath, destinationFilePath);

            if (File.Exists(destinationFilePath))
                return;

            File.Copy(sourceFilePath, destinationFilePath);
        }

        public string GetRealFilePath(FileModel file)
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

            // Директория не существует?
            if (!Directory.Exists(fileFolder))
                Directory.CreateDirectory(fileFolder);

            return filePath.Replace(documentsFolderPath, "");
        }
    }
}
