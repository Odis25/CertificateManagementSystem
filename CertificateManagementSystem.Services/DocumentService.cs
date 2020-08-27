using AutoMapper;
using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Services.Interfaces;
using CertificateManagementSystem.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileProvider _fileProvider;
        private readonly IMapper _mapper;

        public DocumentService(ApplicationDbContext context, IFileProvider fileProvider, IMapper mapper)
        {
            _context = context;
            _fileProvider = fileProvider;
            _mapper = mapper;
        }

        public IEnumerable<int> GetYears()
        {
            return _context.Contracts
                .Where(c => c.Documents.Any())
                .Select(c => c.Year)
                .Distinct()
                .OrderBy(y => y);
        }

        public DocumentDTO GetDocumentById(int id)
        {
            var document = _context.Documents
                .Include(d => d.Contract)
                .Include(d => d.Client)
                .Include(d => d.Device)
                    .ThenInclude(d => d.VerificationMethodic)
                .Include(d => d.DocumentFile)
                .FirstOrDefault(d => d.Id == id);

            var result = _mapper.Map<DocumentDTO>(document);
            return result;
        }
        public IEnumerable<DocumentDTO> GetDocumentsByContractId(int contractId)
        {
            var documents = _context.Documents.Where(d => d.Contract.Id == contractId)
                .Include(d => d.Contract)
                .Include(d => d.Client)
                .Include(d => d.Device)
                    .ThenInclude(d => d.VerificationMethodic)
                .Include(d => d.DocumentFile)
                .ToList();

            var result = _mapper.Map<List<DocumentDTO>>(documents);
            return result;
        }

        public IEnumerable<ContractDTO> GetContracts()
        {
            var contracts = _context.Contracts
                .OrderBy(c => c.ContractNumber)
                .ToList();

            var result = _mapper.Map<List<ContractDTO>>(contracts);
            return result;
        }
        public IEnumerable<ContractDTO> GetContracts(int year)
        {
            var contracts = _context.Contracts
                .Where(c => c.Year == year)
                .OrderBy(c => c.ContractNumber)
                .ToList();

            var result = _mapper.Map<List<ContractDTO>>(contracts);
            return result;
        }

        public IEnumerable<ClientDTO> GetClients()
        {
            var clients = _context.Clients
                .OrderBy(c => c.Name)
                .ToList();

            var result = _mapper.Map<List<ClientDTO>>(clients);
            return result;
        }
        public IEnumerable<DeviceDTO> GetDevices()
        {
            var devices = _context.Devices
                .OrderBy(d => d.Name)
                .ToList();

            var result = _mapper.Map<List<DeviceDTO>>(devices);
            return result;
        }
        public IEnumerable<MethodicDTO> GetMethodics()
        {
            var files = _fileProvider.GetDirectoryContents("");

            return files.Where(f => f.IsDirectory == false).Select(f => new MethodicDTO
            {
                Name = Path.GetFileNameWithoutExtension(f.Name),
                FileName = f.Name
            }).OrderBy(f => f.FileName);
        }
        public async Task Add(DocumentDTO newDocument)
        {
            var client = SetClient(newDocument.Client);
            var contract = SetContract(newDocument.Contract);
            var device = SetDevice(newDocument.Device);

            var doc = _mapper.Map<Document>(newDocument);
            
            doc.Client = client;
            doc.Contract = contract;
            doc.Device = device;

            _context.Documents.Add(doc);

            await _context.SaveChangesAsync();
        }
        public async Task Edit(DocumentDTO documentDTO)
        {
            var document = _context.Documents
                //.Include(d => d.Contract)
                //.Include(d => d.Client)
                //.Include(d => d.Device)
                //    .ThenInclude(dev => dev.VerificationMethodic)
                .FirstOrDefault(d => d.Id == documentDTO.Id);

            var client = SetClient(documentDTO.Client);
            var contract = SetContract(documentDTO.Contract);
            var device = SetDevice(documentDTO.Device);
            var methodic = SetMethodic(documentDTO.Device.VerificationMethodic);

            document.Client = client;
            document.Contract = contract;
            document.Device = device;
            document.Device.VerificationMethodic = methodic;
            document.Device.RegistrationNumber = documentDTO.Device.RegistrationNumber;
            document.Device.Type = documentDTO.Device.Type;
            document.UpdatedBy = documentDTO.UpdatedBy;
            document.UpdatedOn = documentDTO.UpdatedOn;

            try
            {
                _context.Update(document);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
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

        // Приватные методы
        private Contract SetContract(ContractDTO dto)
        {
            var contract = _context.Contracts
                .FirstOrDefault(c => c.ContractNumber == dto.ContractNumber && c.Year == dto.Year)
                ?? new Contract
                {
                    Year = dto.Year,
                    ContractNumber = dto.ContractNumber
                };

            return contract;
        }
        private Client SetClient(ClientDTO dto)
        {
            var client = _context.Clients
                .FirstOrDefault(c => c.Name == dto.Name && c.ExploitationPlace == dto.ExploitationPlace)
                ?? new Client
                {
                    Name = dto.Name,
                    ExploitationPlace = dto.ExploitationPlace
                };

            return client;
        }
        private Device SetDevice(DeviceDTO dto)
        {
            var device = _context.Devices
                .Include(d => d.VerificationMethodic)
                .FirstOrDefault(d => d.Name == dto.Name && d.SerialNumber == dto.SerialNumber)
                ?? new Device 
                {
                    Name = dto.Name,
                    Type = dto.Type,
                    SerialNumber = dto.SerialNumber,
                    RegistrationNumber = dto.RegistrationNumber
                };
            device.VerificationMethodic = SetMethodic(dto.VerificationMethodic);

            return device;
        }
        private Methodic SetMethodic(MethodicDTO dto)
        {
            var methodic = _context.VerificationMethodics
                .FirstOrDefault(m => m.FileName == dto.FileName)
                ?? new Methodic 
                {
                    FileName = dto.FileName,
                    Name = Path.GetFileNameWithoutExtension(dto.FileName)
                };

            return methodic;
        }
    }
}
