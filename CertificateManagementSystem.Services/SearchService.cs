using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CertificateManagementSystem.Services
{
    public class SearchService : ISearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Document> Find(SearchRequest searchRequest)
        {
            var result = new List<Document>();

            if (searchRequest.IsDocumentNumber)
                result.AddRange(SearchInDocumentNumber(searchRequest.SearchQuery));
            if (searchRequest.IsYear)
                result.AddRange(SearchInYear(searchRequest.SearchQuery));
            if (searchRequest.IsContractNumber)
                result.AddRange(SearchInContractNumber(searchRequest.SearchQuery));
            if (searchRequest.IsClientName)
                result.AddRange(SearchInClientName(searchRequest.SearchQuery));
            if (searchRequest.IsExploitationPlace)
                result.AddRange(SearchInExploitationPlace(searchRequest.SearchQuery));
            if (searchRequest.IsVerificationMethodic)
                result.AddRange(SearchInVerificationMethodic(searchRequest.SearchQuery));
            if (searchRequest.IsRegisterNumber)
                result.AddRange(SearchInRegisterNumber(searchRequest.SearchQuery));
            if (searchRequest.IsDeviceType)
                result.AddRange(SearchInDeviceType(searchRequest.SearchQuery));
            if (searchRequest.IsDeviceName)
                result.AddRange(SearchInDeviceName(searchRequest.SearchQuery));
            if (searchRequest.IsSerialNumber)
                result.AddRange(SearchInSerialNumber(searchRequest.SearchQuery));

            var newresult = result.Distinct();

            return result;
        }

        // Искать в номере документа
        private IEnumerable<Document> SearchInDocumentNumber(string query)
        {
            return _context.Documents.Where(d => d.DocumentNumber.Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }
        // Искать в годе заключения договора
        private IEnumerable<Document> SearchInYear(string query)
        {
            return _context.Documents.Where(d => d.Contract.Year.ToString().Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic); 
        }
        // Искать в номере договора
        private IEnumerable<Document> SearchInContractNumber(string query)
        {
            return _context.Documents.Where(d => d.Contract.ContractNumber.Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }
        // Искать в название организации заказчика
        private IEnumerable<Document> SearchInClientName(string query)
        {
            return _context.Documents.Where(d => d.Client.Name.Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }
        // Искать в названии места эксплуатации
        private IEnumerable<Document> SearchInExploitationPlace(string query)
        {
            return _context.Documents.Where(d => d.Client.ExploitationPlace.Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }
        // Искать в группе СИ
        private IEnumerable<Document> SearchInDeviceType(string query)
        {
            return _context.Documents.Where(d => d.Device.Type.Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }
        // Искать в названии СИ
        private IEnumerable<Document> SearchInDeviceName(string query)
        {
            return _context.Documents.Where(d => d.Device.Name.Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }
        // Искать в заводском номере СИ
        private IEnumerable<Document> SearchInSerialNumber(string query)
        {
            return _context.Documents.Where(d => d.Device.SerialNumber.Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }
        // Искать в методике поверки
        private IEnumerable<Document> SearchInVerificationMethodic(string query)
        {
            return _context.Documents.Where(d => d.Device.VerificationMethodic.Name.Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }
        // Искать в номере госреестра
        private IEnumerable<Document> SearchInRegisterNumber(string query)
        {
            return _context.Documents.Where(d => d.Device.VerificationMethodic.RegistrationNumber.Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }
        
    }
}
