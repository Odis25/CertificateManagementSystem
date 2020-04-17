using CertificateManagementSystem.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Data
{
    public interface IDocumentService
    {
        IEnumerable<int> GetYears();

        IEnumerable<Document> GetDocuments();
        IEnumerable<Document> GetDocuments(int year);
        Document GetDocumentById(int id);

        IEnumerable<Contract> GetContracts();
        IEnumerable<Contract> GetContracts(int year);
        Contract GetContractById(int id);
        Contract GetContract(string contractNumber, int year);

        IEnumerable<Client> GetClients();
        Client GetClient(Contract contract);
        Client GetClient(string clientName, string exploitationPlace);

        IEnumerable<Device> GetDevices();
        Device GetDevice(string deviceName, string serialNumber);

        IEnumerable<VerificationMethodic> GetVerificationMethodics();
        VerificationMethodic GetVerificationMethodic(string registrationNumber);

        Task Add(Document newDocument);
       
        bool IsDocumentExist(string documentNumber);
    }
}
