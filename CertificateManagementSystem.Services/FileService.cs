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
          
        }
    }
}
