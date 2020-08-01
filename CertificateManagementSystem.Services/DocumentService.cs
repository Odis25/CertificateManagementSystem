using AutoMapper;
using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Services.Interfaces;
using CertificateManagementSystem.Services.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
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
                .Include(c => c.Documents).ThenInclude(doc => doc.Client)
                .Include(c => c.Documents).ThenInclude(doc => doc.Device)
                .Include(c => c.Documents).ThenInclude(doc => doc.DocumentFile)
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
        public ContractDTO FindContract(string contractNumber, int year)
        {
            var contract = _context.Contracts
                .FirstOrDefault(c => c.ContractNumber == contractNumber && c.Year == year);

            var result = _mapper.Map<ContractDTO>(contract);
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
        public ClientDTO FindClient(string clientName, string exploitationPlace)
        {
            var client = _context.Clients
                .FirstOrDefault(c => c.Name == clientName && c.ExploitationPlace == exploitationPlace);

            var result = _mapper.Map<ClientDTO>(client);
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
        public DeviceDTO FindDevice(string deviceName, string serialNumber)
        {
            var device = _context.Devices
                .Include(d => d.VerificationMethodic)
                .FirstOrDefault(d => d.Name == deviceName && d.SerialNumber == serialNumber);

            var result = _mapper.Map<DeviceDTO>(device);
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
        public MethodicDTO FindMethodic(string methodicFileName)
        {
            var methodic = _context.VerificationMethodics
                .FirstOrDefault(m => m.FileName == methodicFileName);

            var result = _mapper.Map<MethodicDTO>(methodic);
            return result;
        }

        public async Task Add(DocumentDTO newDocument)
        {
            var doc = _mapper.Map<Document>(newDocument);
            _context.Documents.Add(doc);

            await _context.SaveChangesAsync();
        }
        public async Task Edit(DocumentDTO document)
        {
            var doc = _context.Documents.FirstOrDefault(d => d.Id == document.Id);
            var docDTO = _mapper.Map<DocumentDTO>(document);

            var clientDTO = FindClient(document.Client.Name, document.Client.ExploitationPlace);
            var contractDTO = FindContract(document.Contract.ContractNumber, document.Contract.Year);
            var deviceDTO = FindDevice(document.Device.Name, document.Device.SerialNumber);
            var methodicDTO = FindMethodic(document.Device.VerificationMethodic.FileName);

            contractDTO ??= new ContractDTO
            {
                ContractNumber = document.Contract.ContractNumber,
                Year = document.Contract.Year
            };

            clientDTO ??= new ClientDTO
            {
                Name = document.Client.Name,
                ExploitationPlace = document.Client.ExploitationPlace ?? ""
            };

            methodicDTO ??= new MethodicDTO
            {
                Name = document.Device.VerificationMethodic.Name,
                FileName = document.Device.VerificationMethodic.FileName
            };

            deviceDTO ??= new DeviceDTO
            {
                Name = document.Device.Name,
                Type = document.Device.Type,
                SerialNumber = document.Device.SerialNumber
            };


            docDTO.DocumentNumber = document.DocumentNumber;
            docDTO.Client = clientDTO;
            docDTO.Contract = contractDTO;
            docDTO.Device = deviceDTO;
            docDTO.Device.Type = document.Device.Type;
            docDTO.Device.VerificationMethodic = methodicDTO;
            docDTO.Device.RegistrationNumber = document.Device.RegistrationNumber;
            docDTO.UpdatedOn = document.UpdatedOn;
            docDTO.UpdatedBy = document.UpdatedBy;
          
            if (docDTO is CertificateDTO)
            {
                ((CertificateDTO)docDTO).CalibrationDate = ((CertificateDTO)document).CalibrationDate;
                ((CertificateDTO)docDTO).CalibrationExpireDate = ((CertificateDTO)document).CalibrationExpireDate;
            }
            else
            {
                ((FailureNotificationDTO)docDTO).DocumentDate = ((FailureNotificationDTO)document).DocumentDate;
            }

            doc = _mapper.Map<Document>(docDTO);

            _context.Update(doc);

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
