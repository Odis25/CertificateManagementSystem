using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Models.Document;

namespace CertificateManagementSystem.Helpers
{
    public interface IMapper
    {
        Document MapDocumentModel(DocumentCreateModel model);
        Document MapDocumentModel(DocumentEditModel model);
    }
}