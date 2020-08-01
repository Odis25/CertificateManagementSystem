using CertificateManagementSystem.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Services.Interfaces
{
    public interface IDocumentService
    {
        IEnumerable<int> GetYears();

        DocumentDTO GetDocumentById(int id);
        IEnumerable<DocumentDTO> GetDocumentsByContractId(int contractId);

        IEnumerable<ContractDTO> GetContracts();
        IEnumerable<ContractDTO> GetContracts(int year);
        ContractDTO FindContract(string contractNumber, int year);

        IEnumerable<ClientDTO> GetClients();
        ClientDTO FindClient(string clientName, string exploitationPlace);

        IEnumerable<DeviceDTO> GetDevices();
        DeviceDTO FindDevice(string deviceName, string serialNumber);

        IEnumerable<MethodicDTO> GetMethodics();
        MethodicDTO FindMethodic(string methodicName);

        Task Add(DocumentDTO newDocument);
        Task Edit(DocumentDTO document);
        bool IsDocumentExist(string documentNumber);

        Task<int> DocumentsCount();
        Task<int> CertificatesCount();
        Task<int> FailureNotificationsCount();
    }
}
