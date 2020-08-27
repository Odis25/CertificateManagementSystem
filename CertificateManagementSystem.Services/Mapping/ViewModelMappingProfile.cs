using AutoMapper;
using CertificateManagementSystem.Services.Models;
using CertificateManagementSystem.Services.ViewModels.Document;
using System;
using System.IO;

namespace CertificateManagementSystem.Services.Mapping
{
    public class ViewModelMappingProfile : Profile
    {
        public ViewModelMappingProfile()
        {
            CreateMap<DocumentEditModel, DocumentDTO>()
                .ConvertUsing<DocumentEditModelConverter>();

            CreateMap<DocumentCreateModel, DocumentDTO>()
                .ConvertUsing<DocumentCreateModelConverter>();

            //CreateMap<DocumentDTO, DocumentDetailsModel>()
            //    .ForMember(dest => dest.Id, map => map.MapFrom(src => src.Id))
            //    .ForMember(dest => dest.Year, map => map.MapFrom(src => src.Contract.Year))
            //    .ForMember(dest => dest.ContractNumber, map => map.MapFrom(src => src.Contract.ContractNumber))
            //    .ForMember(dest => dest.ClientId, map => map.MapFrom(src => src.Client.Id))
            //    .ForMember(dest => dest.ClientName, map => map.MapFrom(src => src.Client.Name))
            //    .ForMember(dest => dest.ExploitationPlace, map => map.MapFrom(src => src.Client.ExploitationPlace))
            //    .ForMember(dest => dest.DeviceId, map => map.MapFrom(src => src.Device.Id))
            //    .ForMember(dest => dest.DeviceName, map => map.MapFrom(src => src.Device.Name))
            //    .ForMember(dest => dest.DeviceType, map => map.MapFrom(src => src.Device.Type))
            //    .ForMember(dest => dest.SerialNumber, map => map.MapFrom(src => src.Device.SerialNumber))
            //    .ForMember(dest => dest.RegistrationNumber, map => map.MapFrom(src => src.Device.RegistrationNumber))
            //    .ForMember(dest => dest.VerificationMethodic, map => map.MapFrom(src => src.Device.VerificationMethodic.Name))
            //    .ForMember(dest => dest.DocumentNumber, map => map.MapFrom(src => src.DocumentNumber))
            //    .ForMember(dest => dest.DocumentType, map => map.MapFrom(src => src is CertificateDTO ? "Свидетельство о поверке" : "Извещение о непригодности"))
            //    .ForMember(dest => dest.CalibrationDate, map => map.MapFrom(src => src is CertificateDTO ? ((CertificateDTO)src).CalibrationDate.ToString("dd-MM-yyyy") : ""))
            //    .ForMember(dest => dest.CalibrationExpireDate, map => map.MapFrom(src => src is CertificateDTO ? ((CertificateDTO)src).CalibrationExpireDate.ToString("dd-MM-yyyy") : ""))
            //    .ForMember(dest => dest.DocumentDate, map => map.MapFrom(src => src is FailureNotificationDTO ? ((FailureNotificationDTO)src).DocumentDate.ToString("dd-MM-yyyy") : ""))
            //    .ForMember(dest => dest.CreatedOn, map => map.MapFrom(src => src.CreatedOn.ToString("dd-MM-yyyy HH:mm")))
            //    .ForMember(dest => dest.UpdatedOn, map => map.MapFrom(src =>
            //        src.UpdatedOn.HasValue ? ((DateTime)(src.UpdatedOn)).ToString("dd-MM-yyyy HH:mm") : src.CreatedOn.ToString("dd-MM-yyyy HH:mm")))
            //    .ForMember(dest => dest.CreatedBy, map => map.MapFrom(src => src.CreatedBy))
            //    .ForMember(dest => dest.UpdatedBy, map => map.MapFrom(src => src.UpdatedBy ?? src.CreatedBy))
            //    .ForMember(dest => dest.FilePath, map => map.MapFrom(src => Path.Combine("/documentsFolder", src.DocumentFile.Path)));
        }
    }
}
