using AutoMapper;
using CertificateManagementSystem.Services.Models;
using CertificateManagementSystem.Services.ViewModels.Document;
using System;
using System.IO;

namespace CertificateManagementSystem.Services.Mapping
{
    public class DocumentEditModelConverter : ITypeConverter<DocumentEditModel, DocumentDTO>
    {
        public DocumentDTO Convert(DocumentEditModel source, DocumentDTO destination, ResolutionContext context)
        {
            switch (source.DocumentType)
            {
                case Components.DocumentType.Certificate:
                    destination = new CertificateDTO
                    {
                        Id = source.Id,
                        CalibrationDate = (DateTime)source.CalibrationDate,
                        CalibrationExpireDate = (DateTime)source.CalibrationExpireDate,
                        DocumentNumber = source.DocumentNumber,
                        Client = new ClientDTO
                        {
                            Name = source.ClientName,
                            ExploitationPlace = source.ExploitationPlace
                        },
                        Contract = new ContractDTO
                        {
                            Year = source.Year,
                            ContractNumber = source.ContractNumber
                        },
                        Device = new DeviceDTO
                        {
                            Name = source.DeviceName,
                            Type = source.DeviceType,
                            SerialNumber = source.SerialNumber,
                            RegistrationNumber = source.RegistrationNumber,
                            VerificationMethodic = new MethodicDTO
                            {
                                Name = Path.GetFileNameWithoutExtension(source.VerificationMethodic),
                                FileName = source.VerificationMethodic
                            }
                        },
                        UpdatedBy = source.UpdatedBy,
                        UpdatedOn = source.UpdatedOn
                    };
                    
                    return destination;
                case Components.DocumentType.FailureNotification:
                    destination = new FailureNotificationDTO
                    {
                        Id = source.Id,
                        DocumentDate = (DateTime)source.DocumentDate,                        
                        DocumentNumber = source.DocumentNumber,
                        Client = new ClientDTO
                        {
                            Name = source.ClientName,
                            ExploitationPlace = source.ExploitationPlace
                        },
                        Contract = new ContractDTO
                        {
                            Year = source.Year,
                            ContractNumber = source.ContractNumber
                        },
                        Device = new DeviceDTO
                        {
                            Name = source.DeviceName,
                            Type = source.DeviceType,
                            SerialNumber = source.SerialNumber,
                            RegistrationNumber = source.RegistrationNumber,
                            VerificationMethodic = new MethodicDTO
                            {
                                Name = Path.GetFileNameWithoutExtension(source.VerificationMethodic),
                                FileName = source.VerificationMethodic
                            }
                        },
                        UpdatedBy = source.UpdatedBy,
                        UpdatedOn = source.UpdatedOn
                    };
                    return destination;
                default:
                    return null;
            }
        }
    }
}
