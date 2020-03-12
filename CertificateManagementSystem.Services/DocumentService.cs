using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
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
            _context.Devices.Add(newDocument.Device);
            await _context.SaveChangesAsync();
        }

        public IEnumerable<Document> GetAll()
        {
            throw new NotImplementedException();
        }

        public Document GetById(int id)
        {
            throw new NotImplementedException();
        }

        public Client GetClient(string clientName)
        {
            return _context.Clients
                .Include(c=>c.Contracts)
                .Include(c=>c.ExploitationPlaces)
                .FirstOrDefault(c => c.Name == clientName);
        }

        public Contract GetContract(string contractNumber)
        {
            return _context.Contracts
                .Include(c=>c.Client)
                .FirstOrDefault(c => c.ContractNumber == contractNumber);
        }

        public Device GetDevice(string deviceName, string serialNumber)
        {
            return _context.Devices
                .Include(d => d.Contract)
                .FirstOrDefault(d => d.SerialNumber == serialNumber && d.Name == deviceName);
        }
    }
}
