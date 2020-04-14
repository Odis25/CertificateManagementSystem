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

        public async Task Add(Document newDocument)
        {
            _context.Documents.Add(newDocument);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<Client> GetClients()
        {
            return _context.Clients.OrderBy(c => c.Name);
        }
        public IEnumerable<Contract> GetContracts()
        {
            return _context.Contracts.OrderBy(c => c.ContractNumber);
        }

        public IEnumerable<Contract> GetContracts(int year)
        {
            return _context.Contracts.Where(c=>c.Year == year).OrderBy(c => c.ContractNumber);
        }

        public IEnumerable<Device> GetDevices()
        {
            return _context.Devices.OrderBy(d => d.Name);
        }
        public IEnumerable<VerificationMethodic> GetVerificationMethodics()
        {
            return _context.VerificationMethodics.OrderBy(vm => vm.Name);
        }
        public IEnumerable<Document> GetDocuments()
        {
            return _context.Documents
                .Include(d => d.Contract)
                    .ThenInclude(c => c.Client)
                .Include(d => d.Device)
                    .ThenInclude(d => d.VerificationMethodic)
                .Include(d => d.DocumentFile);
        }

        public Document GetDocumentById(int id)
        {
            return GetDocuments().FirstOrDefault(d => d.Id == id);
        }

        public Client GetClient(string clientName, string exploitationPlace)
        {
            return _context.Clients
                .FirstOrDefault(c => c.Name == clientName && c.ExploitationPlace == exploitationPlace);
        }
        public Client GetClient(Contract contract)
        {
            var foundedContract = _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefault(c => c.ContractNumber == contract.ContractNumber && c.Year == contract.Year);
            return foundedContract?.Client;
        }
        public Contract GetContract(string contractNumber, int year)
        {
            return _context.Contracts
                .Include(c => c.Client)
                .Include(c => c.Documents).ThenInclude(cd => cd.Device)
                .FirstOrDefault(c => c.ContractNumber == contractNumber && c.Year == year);
        }
        public Device GetDevice(string deviceName, string serialNumber)
        {
            return _context.Devices
                .Include(d => d.VerificationMethodic)
                .FirstOrDefault(d => d.Name == deviceName && d.SerialNumber == serialNumber);
        }
        public VerificationMethodic GetVerificationMethodic(string registrationNumber)
        {
            return _context.VerificationMethodics.FirstOrDefault(m => m.RegistrationNumber == registrationNumber);
        }

        public bool IsDocumentExist(string documentNumber)
        {
            return _context.Documents.Any(c => c.DocumentNumber == documentNumber);
        }      
    }
}
