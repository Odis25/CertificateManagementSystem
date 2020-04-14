using CertificateManagementSystem.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Data
{
    public interface IDocumentService
    {
        IEnumerable<Document> GetDocuments();
        IEnumerable<Contract> GetContracts();
        IEnumerable<Contract> GetContracts(int year);
        IEnumerable<Client> GetClients();
        IEnumerable<Device> GetDevices();
        IEnumerable<VerificationMethodic> GetVerificationMethodics();

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
