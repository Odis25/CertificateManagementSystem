using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext _context;

        public DocumentService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<int> GetYears()
        {
            return _context.Contracts.Select(c => c.Year).Distinct().OrderBy(y => y);
        }

        public Document GetDocumentById(int id)
        {
            return _context.Documents
                .Include(d => d.Contract)
                .Include(d => d.Client)
                .Include(d => d.Device)
                    .ThenInclude(d => d.VerificationMethodic)
                .Include(d => d.DocumentFile)
                .FirstOrDefault(d => d.Id == id);
        }
        public IEnumerable<Document> GetDocumentsByContractId(int contractId)
        {
            return _context.Documents.Where(d => d.Contract.Id == contractId)
                .Include(d => d.Contract)
                .Include(d => d.Client)
                .Include(d => d.Device)
                    .ThenInclude(d => d.VerificationMethodic)
                .Include(d => d.DocumentFile);
        }

        public IEnumerable<Contract> GetContracts()
        {
            return _context.Contracts
                .OrderBy(c => c.ContractNumber)
                .Include(c => c.Documents).ThenInclude(doc => doc.Client)
                .Include(c => c.Documents).ThenInclude(doc => doc.Device)
                .Include(c => c.Documents).ThenInclude(doc => doc.DocumentFile); ;
        }
        public IEnumerable<Contract> GetContracts(int year)
        {
            return _context.Contracts.Where(c => c.Year == year).OrderBy(c => c.ContractNumber);
        }
        public Contract FindContract(string contractNumber, int year)
        {
            return GetContracts()
                .FirstOrDefault(c => c.ContractNumber == contractNumber && c.Year == year);
        }

        public IEnumerable<Client> GetClients()
        {
            return _context.Clients.OrderBy(c => c.Name);
        }
        public Client FindClient(string clientName, string exploitationPlace)
        {
            return _context.Clients
                .FirstOrDefault(c => c.Name == clientName && c.ExploitationPlace == exploitationPlace);
        }

        public IEnumerable<Device> GetDevices()
        {
            return _context.Devices.OrderBy(d => d.Name);
        }
        public Device FindDevice(string deviceName, string serialNumber)
        {
            return _context.Devices
                .Include(d => d.VerificationMethodic)
                .FirstOrDefault(d => d.Name == deviceName && d.SerialNumber == serialNumber);
        }

        public IEnumerable<VerificationMethodic> GetVerificationMethodics()
        {
            return _context.VerificationMethodics.OrderBy(vm => vm.Name);
        }
        public VerificationMethodic FindVerificationMethodic(string registrationNumber)
        {
            return _context.VerificationMethodics.FirstOrDefault(m => m.RegistrationNumber == registrationNumber);
        }

        public async Task Add(Document newDocument)
        {
            _context.Documents.Add(newDocument);
            await _context.SaveChangesAsync();
        }

        public bool IsDocumentExist(string documentNumber)
        {
            return _context.Documents.Any(c => c.DocumentNumber == documentNumber);
        }

        public async Task<int> DocumentsCount()
        {
            return await _context.Documents.CountAsync();
        }
        public async Task<int> CertificatesCount()
        {
            return await _context.Documents.OfType<Certificate>().CountAsync();
        }
        public async Task<int> FailureNotificationsCount()
        {
            return await _context.Documents.OfType<FailureNotification>().CountAsync();
        }
    }
}
