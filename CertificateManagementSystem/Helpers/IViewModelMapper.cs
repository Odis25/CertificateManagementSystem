using CertificateManagementSystem.Models.Document;
using CertificateManagementSystem.Services.Models;

namespace CertificateManagementSystem.Helpers
{
    public interface IViewModelMapper
    {
        DocumentDTO MapDocumentModel(DocumentCreateModel model);
        DocumentDTO MapDocumentModel(DocumentEditModel model);
    }
}