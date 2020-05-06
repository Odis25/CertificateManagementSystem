using CertificateManagementSystem.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Data
{
    public interface IDocumentService
    {
        IEnumerable<int> GetYears();

        Document GetDocumentById(int id);
        IEnumerable<Document> GetDocumentsByContractId(int contractId);

        IEnumerable<Contract> GetContracts();
        IEnumerable<Contract> GetContracts(int year);
        Contract FindContract(string contractNumber, int year);

        IEnumerable<Client> GetClients();
        Client FindClient(string clientName, string exploitationPlace);

        IEnumerable<Device> GetDevices();
        Device FindDevice(string deviceName, string serialNumber);

        IEnumerable<VerificationMethodic> GetVerificationMethodics();
        VerificationMethodic FindVerificationMethodic(string registrationNumber);

        Task Add(Document newDocument);
       
        bool IsDocumentExist(string documentNumber);

        Task<int> DocumentsCount();
        Task<int> CertificatesCount();
        Task<int> FailureNotificationsCount();
    }
}
