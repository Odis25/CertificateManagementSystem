using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Models.Document;
using CertificateManagementSystem.Services.Components;
using CertificateManagementSystem.Services.Interfaces;
using System;
using System.IO;

namespace CertificateManagementSystem.Helpers
{
    public class Mapper : IMapper
    {
        private readonly IFileService _fileService;
        private readonly IDocumentService _documentService;

        public Mapper(IFileService fileService, IDocumentService documentService)
        {
            _fileService = fileService;
            _documentService = documentService;
        }
        public Document MapDocumentModel(DocumentCreateModel model)
        {
            switch (model.DocumentType)
            {
                case DocumentType.Certificate:
                    return new Certificate
                    {
                        Contract = CreateContract(model),
                        Client = CreateClient(model),
                        Device = CreateDevice(model),
                        DocumentFile = CreateFilePath(model),
                        CalibrationDate = model.CalibrationDate,
                        CalibrationExpireDate = model.CalibrationExpireDate,
                        DocumentNumber = model.DocumentNumber,
                        CreatedBy = model.CreatedBy,
                        CreatedOn = model.CreatedOn
                    };

                case DocumentType.FailureNotification:
                    return new FailureNotification
                    {
                        Contract = CreateContract(model),
                        Client = CreateClient(model),
                        Device = CreateDevice(model),
                        DocumentFile = CreateFilePath(model),
                        DocumentDate = model.DocumentDate,
                        DocumentNumber = model.DocumentNumber,
                        CreatedBy = model.CreatedBy,
                        CreatedOn = model.CreatedOn
                    };

                default:
                    return null;
            }
        }

        public Document MapDocumentModel(DocumentEditModel model)
        {
            switch (model.DocumentType)
            {
                case DocumentType.Certificate:
                    return new Certificate
                    {
                        Id = model.Id,
                        CalibrationDate = (DateTime)model.CalibrationDate,
                        CalibrationExpireDate = (DateTime)model.CalibrationExpireDate,
                        DocumentNumber = model.DocumentNumber,
                        Client = new Client
                        {
                            Name = model.ClientName,
                            ExploitationPlace = model.ExploitationPlace
                        },
                        Contract = new Contract
                        {
                            Year = model.Year,
                            ContractNumber = model.ContractNumber
                        },
                        Device = new Device
                        {
                            Name = model.DeviceName,
                            Type = model.DeviceType,
                            SerialNumber = model.SerialNumber,
                            RegistrationNumber = model.RegistrationNumber,
                            VerificationMethodic = new Methodic { FileName = model.VerificationMethodic }
                        },
                        UpdatedBy = model.UpdatedBy,
                        UpdatedOn = model.UpdatedOn
                    };

                case DocumentType.FailureNotification:
                    return new FailureNotification
                    {
                        Id = model.Id,
                        DocumentDate = (DateTime)model.DocumentDate,
                        DocumentNumber = model.DocumentNumber,
                        Client = new Client
                        {
                            Name = model.ClientName,
                            ExploitationPlace = model.ExploitationPlace
                        },
                        Contract = new Contract
                        {
                            Year = model.Year,
                            ContractNumber = model.ContractNumber
                        },
                        Device = new Device
                        {
                            Name = model.DeviceName,
                            Type = model.DeviceType,
                            SerialNumber = model.SerialNumber,
                            RegistrationNumber = model.RegistrationNumber,
                            VerificationMethodic = new Methodic { FileName = model.VerificationMethodic }
                        },
                        UpdatedBy = model.UpdatedBy,
                        UpdatedOn = model.UpdatedOn
                    };

                default:
                    return null;
            }
        }

        // Формируем модель файла документа
        private FileModel CreateFilePath(DocumentCreateModel model)
        {
            var year = model.Year.ToString();
            var contract = model.ContractNumber.ReplaceInvalidChars('-') ?? "";
            var deviceType = model.DeviceType.ReplaceInvalidChars('-');
            var deviceName = model.DeviceName.ReplaceInvalidChars('-');
            var docType = model.DocumentType == DocumentType.Certificate ? "Свидетельства" : "Извещения о непригодности";

            var extension = Path.GetExtension(model.DocumentFile?.FileName);
            var fileName = deviceType + "_" + deviceName + extension;
            var filePath = Path.Combine(year, contract, docType, fileName);

            var file = new FileModel
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
        // Формируем нового заказчика
        private Client CreateClient(DocumentCreateModel model)
        {
            return _documentService.FindClient(model.ClientName, model.ExploitationPlace) ??
                new Client
                {
                    Name = model.ClientName?.Trim().Capitalize(),
                    ExploitationPlace = model.ExploitationPlace?.Trim().Capitalize()
                };
        }
        // Формируем новый договор
        private Contract CreateContract(DocumentCreateModel model)
        {
            return _documentService.FindContract(model.ContractNumber, model.Year)
                ?? new Contract
                {
                    Year = model.Year,
                    ContractNumber = model.ContractNumber?.Trim()
                };
        }
        // Формируем новую метоздику поверки
        private Methodic CreateMethodic(DocumentCreateModel model)
        {
            var methodic = _documentService.FindMethodic(model.VerificationMethodic) ??
                new Methodic
                {
                    Name = Path.GetFileNameWithoutExtension(model.VerificationMethodic),
                    FileName = model.VerificationMethodic
                };

            return methodic;
        }
        // Формируем новое устройство
        private Device CreateDevice(DocumentCreateModel model)
        {
            var device = _documentService.FindDevice(model.DeviceName, model.SerialNumber);
            var methodic = CreateMethodic(model);

            device ??= new Device
            {
                Name = model.DeviceName?.Trim(),
                Type = model.DeviceType?.Trim(),
                SerialNumber = model.SerialNumber?.Trim(),
                RegistrationNumber = model.RegistrationNumber?.Trim(),
                VerificationMethodic = methodic
            };
            return device;
        }

    }
}
