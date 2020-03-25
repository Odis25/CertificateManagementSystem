﻿using CertificateManagementSystem.Data;
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

        public IEnumerable<Client> GetAllClients()
        {
            return _context.Clients.OrderBy(c => c.Name);
        }

        public IEnumerable<Contract> GetAllContracts()
        {
            return _context.Contracts.OrderBy(c => c.ContractNumber);
        }

        public IEnumerable<Device> GetAllDevices()
        {
            return _context.Devices.OrderBy(d => d.Name);
        }

        public IEnumerable<VerificationMethodic> GetAllVerificationMethodics()
        {
            return _context.VerificationMethodics.OrderBy(vm => vm.Name);
        }

        public IEnumerable<Document> GetAllDocuments()
        {
            return _context.Documents;
        }

        public Document GetDocumentById(int id)
        {
            return GetAllDocuments().FirstOrDefault(d => d.Id == id);
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
                .Include(c => c.Devices)
                .FirstOrDefault(c => c.ContractNumber == contractNumber && c.Year == year);
        }

        public Device GetDevice(string deviceName, string serialNumber)
        {
            return _context.Devices
                .Include(d => d.Contracts)
                .ThenInclude(c => c.Client)
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
