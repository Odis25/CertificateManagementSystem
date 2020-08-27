using AutoMapper;
using CertificateManagementSystem.Services.Components;
using CertificateManagementSystem.Services.Interfaces;
using CertificateManagementSystem.Services.Models;
using CertificateManagementSystem.Services.ViewModels.Document;
using System.IO;

namespace CertificateManagementSystem.Services.Mapping
{
    public class DocumentCreateModelConverter : ITypeConverter<DocumentCreateModel, DocumentDTO>
    {
        private readonly IFileService _fileService;

        public DocumentCreateModelConverter(IFileService fileService)
        {
            _fileService = fileService;
        }
        public DocumentDTO Convert(DocumentCreateModel source, DocumentDTO destination, ResolutionContext context)
        {
            switch (source.DocumentType)
            {
                case DocumentType.Certificate:
                    destination = new CertificateDTO
                    {
                        CalibrationDate = source.CalibrationDate,
                        CalibrationExpireDate = source.CalibrationExpireDate
                    };
                    break;
                case DocumentType.FailureNotification:
                    destination = new FailureNotificationDTO
                    {
                        DocumentDate = source.DocumentDate
                    };
                    break;
                default:
                    destination = new CertificateDTO();
                    break;
            }
            destination.Contract = new ContractDTO
            {
                Year = source.Year,
                ContractNumber = source.ContractNumber?.Trim()
            };
            destination.Client = new ClientDTO
            {
                Name = source.ClientName?.Trim().Capitalize(),
                ExploitationPlace = source.ExploitationPlace?.Trim().Capitalize()
            };
            destination.Device = new DeviceDTO
            {
                Name = source.DeviceName?.Trim(),
                Type = source.DeviceType?.Trim(),
                SerialNumber = source.SerialNumber?.Trim(),
                RegistrationNumber = source.RegistrationNumber?.Trim(),
                VerificationMethodic = new MethodicDTO
                {
                    Name = Path.GetFileNameWithoutExtension(source.VerificationMethodic),
                    FileName = source.VerificationMethodic
                }
            };
            destination.DocumentFile = CreateFilePath(source);
            destination.DocumentNumber = source.DocumentNumber;
            destination.CreatedBy = source.CreatedBy;
            destination.CreatedOn = source.CreatedOn;

            return destination;
        }

        // Формируем модель файла документа
        private FileModelDTO CreateFilePath(DocumentCreateModel model)
        {
            var year = model.Year.ToString();
            var contract = model.ContractNumber.ReplaceInvalidChars('-') ?? "";
            var deviceType = model.DeviceType.ReplaceInvalidChars('-');
            var deviceName = model.DeviceName.ReplaceInvalidChars('-');
            var docType = model.DocumentType == DocumentType.Certificate ? "Свидетельства" : "Извещения о непригодности";

            var extension = Path.GetExtension(model.DocumentFile?.FileName);
            var fileName = deviceType + "_" + deviceName + extension;
            var filePath = Path.Combine(year, contract, docType, fileName);

            var file = new FileModelDTO
            {
                Size = model.DocumentFile?.Length ?? 0,
                ContentType = model.DocumentFile?.ContentType,
                Path = filePath
            };

            // Актуализируем путь к файлу
            file.Path = _fileService.ActualizeFilePath(file);
            file.FileName = Path.GetFileName(file.Path);

            return file;
        }
    }
}
