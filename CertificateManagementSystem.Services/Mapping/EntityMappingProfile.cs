using AutoMapper;
using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Services.Models;

namespace CertificateManagementSystem.Services.Mapping
{
    public class EntityMappingProfile : Profile
    {
        public EntityMappingProfile()
        {
            CreateMap<Document, DocumentDTO>().ReverseMap();
            CreateMap<Client, ClientDTO>().ReverseMap();
            CreateMap<Contract, ContractDTO>().ReverseMap();
            CreateMap<Device, DeviceDTO>().ReverseMap();
            CreateMap<Certificate, CertificateDTO>().ReverseMap();
            CreateMap<FailureNotification, FailureNotificationDTO>().ReverseMap();
            CreateMap<FileModel, FileModelDTO>().ReverseMap();
            CreateMap<Methodic, MethodicDTO>().ReverseMap();
            CreateMap<SearchRequest, SearchRequestDTO>().ReverseMap();
        }
    }
}
