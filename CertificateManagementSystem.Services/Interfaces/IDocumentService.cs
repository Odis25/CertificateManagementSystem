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
        IEnumerable<ClientDTO> GetClients();
        IEnumerable<DeviceDTO> GetDevices();
        IEnumerable<MethodicDTO> GetMethodics();

        Task Add(DocumentDTO newDocument);
        Task Edit(DocumentDTO document);
        bool IsDocumentExist(string documentNumber);

        Task<int> DocumentsCount();
        Task<int> CertificatesCount();
        Task<int> FailureNotificationsCount();
    }
}
