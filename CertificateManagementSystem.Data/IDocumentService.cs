using CertificateManagementSystem.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Data
{
    public interface IDocumentService
    {
        IEnumerable<Document> GetAllDocuments();
        IEnumerable<Contract> GetAllContracts();
        IEnumerable<Client> GetAllClients();
        IEnumerable<Device> GetAllDevices();
        IEnumerable<VerificationMethodic> GetAllVerificationMethodics();

        Document GetDocumentById(int id);
        Task Add(Document newDocument);

        Device GetDevice(string deviceName, string serialNumber);
        Contract GetContract(string contractNumber, int year);
        Client GetClient(string clientName, string exploitationPlace);
        Client GetClient(Contract contract);
        VerificationMethodic GetVerificationMethodic(string registrationNumber);
        
        bool IsDocumentExist(string documentNumber);
    }
}
