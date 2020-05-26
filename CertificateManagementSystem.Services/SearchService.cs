using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

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

            var keyWords = searchRequest.SearchQuery.ToLower().Split(' ');

            foreach (var word in keyWords)
            {
                if (searchRequest.IsDocumentNumber)
                    result.AddRange(SearchInDocumentNumber(word));
                if (searchRequest.IsYear)
                    result.AddRange(SearchInYear(word));
                if (searchRequest.IsContractNumber)
                    result.AddRange(SearchInContractNumber(word));
                if (searchRequest.IsClientName)
                    result.AddRange(SearchInClientName(word));
                if (searchRequest.IsExploitationPlace)
                    result.AddRange(SearchInExploitationPlace(word));
                if (searchRequest.IsVerificationMethodic)
                    result.AddRange(SearchInVerificationMethodic(word));
                if (searchRequest.IsRegisterNumber)
                    result.AddRange(SearchInRegisterNumber(word));
                if (searchRequest.IsDeviceType)
                    result.AddRange(SearchInDeviceType(word));
                if (searchRequest.IsDeviceName)
                    result.AddRange(SearchInDeviceName(word));
                if (searchRequest.IsSerialNumber)
                    result.AddRange(SearchInSerialNumber(word));
            }

            var filtredResult = result.Where(d => keyWords.All(word =>
            {
                if (searchRequest.IsDocumentNumber)
                    if (d.DocumentNumber?.ToLower().Contains(word) ?? false)
                        return true;
                if (searchRequest.IsYear)
                    if (d.Contract.Year.ToString().ToLower().Contains(word))
                        return true;
                if (searchRequest.IsContractNumber)
                    if (d.Contract.ContractNumber.ToString().ToLower().Contains(word))
                        return true;
                if (searchRequest.IsClientName)
                    if (d.Client.Name.ToString().ToLower().Contains(word))
                        return true;
                if (searchRequest.IsExploitationPlace)
                    if (d.Client.ExploitationPlace.ToString().ToLower().Contains(word))
                        return true;
                if (searchRequest.IsDeviceType)
                    if (d.Device.Type.ToString().ToLower().Contains(word))
                        return true;
                if (searchRequest.IsDeviceName)
                    if (d.Device.Name.ToString().ToLower().Contains(word))
                        return true;
                if (searchRequest.IsSerialNumber)
                    if (d.Device.SerialNumber.ToString().ToLower().Contains(word))
                        return true;
                if (searchRequest.IsVerificationMethodic)
                    if (d.Device.VerificationMethodic?.Name.ToString().ToLower().Contains(word) ?? false)
                        return true;
                if (searchRequest.IsRegisterNumber)
                    if (d.Device.VerificationMethodic?.RegistrationNumber.ToString().ToLower().Contains(word) ?? false)
                        return true;

                return false;
            }));

            return filtredResult;
        }

        // Искать в номере документа
        private IEnumerable<Document> SearchInDocumentNumber(string query)
        {
            return _context.Documents.Where(d => d.DocumentNumber.ToLower().Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }
        // Искать в годе заключения договора
        private IEnumerable<Document> SearchInYear(string query)
        {
            return _context.Documents.Where(d => d.Contract.Year.ToString().ToLower().Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }
        // Искать в номере договора
        private IEnumerable<Document> SearchInContractNumber(string query)
        {
            return _context.Documents.Where(d => d.Contract.ContractNumber.ToLower().Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }
        // Искать в название организации заказчика
        private IEnumerable<Document> SearchInClientName(string query)
        {
            return _context.Documents.Where(d => d.Client.Name.ToLower().Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }
        // Искать в названии места эксплуатации
        private IEnumerable<Document> SearchInExploitationPlace(string query)
        {
            return _context.Documents.Where(d => d.Client.ExploitationPlace.ToLower().Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }
        // Искать в группе СИ
        private IEnumerable<Document> SearchInDeviceType(string query)
        {
            return _context.Documents.Where(d => d.Device.Type.ToLower().Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }
        // Искать в названии СИ
        private IEnumerable<Document> SearchInDeviceName(string query)
        {
            return _context.Documents.Where(d => d.Device.Name.ToLower().Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }
        // Искать в заводском номере СИ
        private IEnumerable<Document> SearchInSerialNumber(string query)
        {
            return _context.Documents.Where(d => d.Device.SerialNumber.ToLower().Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }
        // Искать в методике поверки
        private IEnumerable<Document> SearchInVerificationMethodic(string query)
        {
            return _context.Documents.Where(d => d.Device.VerificationMethodic.Name.ToLower().Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }
        // Искать в номере госреестра
        private IEnumerable<Document> SearchInRegisterNumber(string query)
        {
            return _context.Documents.Where(d => d.Device.VerificationMethodic.RegistrationNumber.ToLower().Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }

    }
}
