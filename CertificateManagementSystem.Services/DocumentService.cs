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

        public IEnumerable<Document> GetAll()
        {
            return _context.Documents;
        }

        public Document GetById(int id)
        {
            return GetAll().FirstOrDefault(d => d.Id == id);
        }

        public Client GetClient(string clientName, string exploitationPlace)
        {
            return _context.Clients
                .FirstOrDefault(c => c.Name == clientName && c.ExploitationPlace == exploitationPlace);
        }

        public Contract GetContract(string contractNumber, int year)
        {
            return _context.Contracts
                .Include(c => c.Client)
                .FirstOrDefault(c => c.ContractNumber == contractNumber && c.Year == year);
        }

        public Device GetDevice(string deviceName, string serialNumber, string contractNumber, int year)
        {
            return _context.Devices
                .Include(d => d.Contract)
                .ThenInclude(c => c.Client)
                .Include(d => d.VerificationMethodic)
                .FirstOrDefault(d => d.SerialNumber == serialNumber &&
                    d.Name == deviceName &&
                    d.Contract.ContractNumber == contractNumber &&
                    d.Contract.Year == year);
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
