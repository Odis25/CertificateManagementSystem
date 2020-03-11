using CertificateManagementSystem.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Data
{
    public interface IDocumentService
    {
        IEnumerable<Document> GetAll();
        Document GetById(int id);
        Task Add(Document newDocument);
    }
}
