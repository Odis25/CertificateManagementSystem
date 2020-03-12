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

        Device GetDevice(string deviceName, string serialNumber);
        Contract GetContract(string contractNumber);
        Client GetClient(string clientName);
    }
}
