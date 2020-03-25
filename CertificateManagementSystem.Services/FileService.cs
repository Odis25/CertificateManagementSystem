using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using Microsoft.Extensions.Configuration;
using CertificateManagementSystem.Services.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Services
{
    public class FileService : IFileService
    {
        private readonly IConfiguration _configuration;

        public FileService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task CreateFile(Document document)
        {
            //var documentPath = document.FilePath;
            var isCertificate = document is Certificate;

            var basePath = _configuration.GetSection("Paths:DocumentFolder").Value;

            var year = document.Device.Contract.Year.ToString();
            var contract = document.Device.Contract.ContractNumber.ReplaceInvalidChars('-');
            var deviceType = document.Device.Type.ReplaceInvalidChars('-'); 
            var deviceName = document.Device.Name.ReplaceInvalidChars('-'); 
            var type = isCertificate ? "Свидетельства" : "Извещения о непригодности";

            var fileName = deviceType + "_" + deviceName;
            var pathArray = new[] { basePath, year, contract, type, fileName };
            
            var filePath = Path.Combine(pathArray);
           
        }
    }
}
