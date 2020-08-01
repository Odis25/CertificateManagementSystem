using AutoMapper;
using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Services.Interfaces;
using CertificateManagementSystem.Services.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CertificateManagementSystem.Services
{
    public class SearchService : ISearchService
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public SearchService(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<DocumentDTO> Find(SearchRequestDTO searchRequest)
        {
            var query = searchRequest.SearchQuery.ToLower();
            var dates = ExtractDatePatterns(ref query);

            searchRequest.SearchQuery = query;

            // Поиск по ключевым словам
            var documents = SearchByKeywords(searchRequest);
            // Поиск по датам
            if (dates.Length > 0)
            {
                if (documents.Any())
                    documents = documents.Intersect(SearchByDatePatterns(dates));
                else
                    documents = SearchByDatePatterns(dates);
            }

            var result = _mapper.Map<List<DocumentDTO>>(documents);
            return result;
        }

        private IEnumerable<Document> SearchByKeywords(SearchRequestDTO request)
        {
            var keyWords = request.SearchQuery.Split(' ');
            var result = new List<Document>();

            foreach (var word in keyWords)
            {
                if (request.IsDocumentNumber)
                    result.AddRange(SearchInDocumentNumber(word));
                if (request.IsYear)
                    result.AddRange(SearchInYear(word));
                if (request.IsContractNumber)
                    result.AddRange(SearchInContractNumber(word));
                if (request.IsClientName)
                    result.AddRange(SearchInClientName(word));
                if (request.IsExploitationPlace)
                    result.AddRange(SearchInExploitationPlace(word));
                if (request.IsVerificationMethodic)
                    result.AddRange(SearchInVerificationMethodic(word));
                if (request.IsRegisterNumber)
                    result.AddRange(SearchInRegisterNumber(word));
                if (request.IsDeviceType)
                    result.AddRange(SearchInDeviceType(word));
                if (request.IsDeviceName)
                    result.AddRange(SearchInDeviceName(word));
                if (request.IsSerialNumber)
                    result.AddRange(SearchInSerialNumber(word));
                result.AddRange(SearchInDocumentType(word));
            }

            var filtredResult = result.Where(d => keyWords.All(word =>
            {
                if (request.IsDocumentNumber)
                    if (d.DocumentNumber?.ToLower().Contains(word) ?? false)
                        return true;
                if (request.IsYear)
                    if (d.Contract.Year.ToString().ToLower().Contains(word))
                        return true;
                if (request.IsContractNumber)
                    if (d.Contract.ContractNumber.ToLower().Contains(word))
                        return true;
                if (request.IsClientName)
                    if (d.Client.Name.ToLower().Contains(word))
                        return true;
                if (request.IsExploitationPlace)
                    if (d.Client.ExploitationPlace?.ToLower().Contains(word) ?? false)
                        return true;
                if (request.IsDeviceType)
                    if (d.Device.Type.ToLower().Contains(word))
                        return true;
                if (request.IsDeviceName)
                    if (d.Device.Name.ToLower().Contains(word))
                        return true;
                if (request.IsSerialNumber)
                    if (d.Device.SerialNumber.ToLower().Contains(word))
                        return true;
                if (request.IsVerificationMethodic)
                    if (d.Device.VerificationMethodic?.Name?.ToLower().Contains(word) ?? false)
                        return true;
                if (request.IsRegisterNumber)
                    if (d.Device.RegistrationNumber?.ToLower().Contains(word) ?? false)
                        return true;
                if ("свидетельство о поверке".Contains(word))
                    if (d is Certificate)
                        return true;
                if ("извещение о непригодности".Contains(word))
                    if (d is FailureNotification)
                        return true;

                return false;
            }));

            return filtredResult;
        }

        // Поиск по дате
        private IEnumerable<Document> SearchByDatePatterns(string[] dates)
        {
            var datePattern = new Regex(@"\d{2}.\d{2}.\d{4}");
            var rangePattern = new Regex(@"\d{2}.\d{2}.\d{4}\s*-\s*\d{2}.\d{2}.\d{4}");
            var result = new List<Document>();

            foreach (var date in dates)
            {
                if (rangePattern.IsMatch(date))
                {
                    var startRange = Convert.ToDateTime(datePattern.Matches(date)[0].Value);
                    var endRange = Convert.ToDateTime(datePattern.Matches(date)[1].Value);

                    var certificates = _context.Documents.OfType<Certificate>().Where(d =>
                        d.CalibrationDate >= startRange &&
                        d.CalibrationExpireDate <= endRange);

                    var notifications = _context.Documents.OfType<FailureNotification>().Where(d =>
                        d.DocumentDate >= startRange &&
                        d.DocumentDate <= endRange);

                    result.AddRange(certificates);
                    result.AddRange(notifications);
                }
                else
                {
                    var targetDate = Convert.ToDateTime(date);

                    var certificates = _context.Documents.OfType<Certificate>().Where(d =>
                        d.CalibrationDate == targetDate || d.CalibrationExpireDate == targetDate);

                    var notifications = _context.Documents.OfType<FailureNotification>().Where(d =>
                        d.DocumentDate == targetDate);

                    result.AddRange(certificates);
                    result.AddRange(notifications);
                }
            }

            return result;
        }

        // Извлекаем из строки запроса патерны дат если они там есть
        private string[] ExtractDatePatterns(ref string query)
        {
            var datePattern = new Regex(@"\d{2}.\d{2}.\d{4}");
            var rangePattern = new Regex(@"\d{2}.\d{2}.\d{4}\s*-\s*\d{2}.\d{2}.\d{4}");
            var result = new List<string>();

            // В строке запроса есть диапазоны дат
            var rangeMatches = rangePattern.Matches(query);

            if (rangeMatches.Count > 0)
            {
                foreach (Match match in rangeMatches)
                {
                    query = query.Remove(query.IndexOf(match.Value), match.Value.Length);
                    var res = Regex.Replace(match.Value, @"[^0-9\s-]", ".");
                    result.Add(res);
                }
            }

            // В строке запроса есть одиночные даты
            var dateMatches = datePattern.Matches(query);
            if (dateMatches.Count > 0)
            {
                foreach (Match match in dateMatches)
                {
                    query = query.Remove(query.IndexOf(match.Value), match.Value.Length);
                    var res = Regex.Replace(match.Value, @"[^0-9\s-]", ".");
                    result.Add(res);
                }
            }

            return result.ToArray();
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
            return _context.Documents.Where(d => d.Device.RegistrationNumber.ToLower().Contains(query))
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
        }

        private IEnumerable<Document> SearchInDocumentType(string query)
        {
            if ("свидетельство о поверке".Contains(query))
            {
                return _context.Documents.OfType<Certificate>()
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
            }
            else if ("извещение о непригодности".Contains(query))
            {
                return _context.Documents.OfType<FailureNotification>()
                .Include(d => d.Client)
                .Include(d => d.Contract)
                .Include(d => d.DocumentFile)
                .Include(d => d.Device)
                .ThenInclude(dev => dev.VerificationMethodic);
            }
            return new List<Document>();
        }

    }
}
