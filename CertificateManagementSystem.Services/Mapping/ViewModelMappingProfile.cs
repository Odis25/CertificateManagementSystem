using AutoMapper;
using CertificateManagementSystem.Services.Models;
using CertificateManagementSystem.Services.ViewModels.Document;

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
        }
    }
}
