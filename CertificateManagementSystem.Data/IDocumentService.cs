using CertificateManagementSystem.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Data
{
    public interface IDocumentService
    {
        IEnumerable<Document> GetAll();
        Document GetById(int id);
        Task<int> Add(Document newDocument);

        Device GetDevice(string deviceName, string serialNumber, string contractNumber, int year);
        Contract GetContract(string contractNumber, int year);
        Client GetClient(string clientName, string exploitationPlace);
        VerificationMethodic GetVerificationMethodic(string registrationNumber);
    }
}
