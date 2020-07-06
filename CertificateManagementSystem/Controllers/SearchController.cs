using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Models.Document;
using CertificateManagementSystem.Models.Search;
using CertificateManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace CertificateManagementSystem.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchService _search;

        public SearchController(ISearchService search)
        {
            _search = search;
        }

        public IActionResult Index(SearchResultModel model)
        {
            return View(model);
        }

        [HttpPost]
        public IActionResult Find(SearchModel searchModel)
        {
            var searchRequest = new SearchRequest
            {
                SearchQuery = searchModel.SearchQuery,
                IsDocumentNumber = searchModel.IsDocumentNumber,
                IsYear = searchModel.IsYear,
                IsContractNumber = searchModel.IsContractNumber,
                IsClientName = searchModel.IsClientName,
                IsExploitationPlace = searchModel.IsExploitationPlace,
                IsVerificationMethodic = searchModel.IsVerificationMethodic,
                IsRegisterNumber = searchModel.IsRegisterNumber,
                IsDeviceType = searchModel.IsDeviceType,
                IsDeviceName = searchModel.IsDeviceName,
                IsSerialNumber = searchModel.IsSerialNumber,
                IsDocumentType = searchModel.IsDocumentType
            };

            var searchResult = _search.Find(searchRequest);
            var documents = searchResult.Select(d => new DocumentListingModel
            {
                Id = d.Id,
                Year = d.Contract.Year,
                ContractId = d.Contract.Id,
                ContractNumber = d.Contract.ContractNumber,
                ClientId = d.Client.Id,
                ClientName = d.Client.Name,
                ExploitationPlace = d.Client.ExploitationPlace,
                DeviceId = d.Device.Id,
                DeviceType = d.Device.Type,
                DeviceName = d.Device.Name,
                SerialNumber = d.Device.SerialNumber,
                RegistrationNumber = d.Device.VerificationMethodic?.RegistrationNumber,
                VerificationMethodic = d.Device.VerificationMethodic?.Name,
                DocumentNumber = d.DocumentNumber,
                CalibrationDate = (d as Certificate)?.CalibrationDate.ToString("dd-MM-yyyy"),
                CalibrationExpireDate = (d as Certificate)?.CalibrationExpireDate.ToString("dd-MM-yyyy"),
                FilePath = d.DocumentFile.Path,
                DocumentType = (d is Certificate) ? "Свидетельство" : "Извещение о непригодности",
                DocumentDate = (d as FailureNotification)?.DocumentDate.ToString("dd-MM-yyyy"),
                CreatedOn = d.CreatedOn.ToString("dd-MM-yyyy hh:mm"),
                UpdatedOn = d.UpdatedOn?.ToString("dd-MM-yyyy hh:mm") ?? d.CreatedOn.ToString("dd-MM-yyyy hh:mm"),
                CreatedBy = d.CreatedBy,
                UpdatedBy = d.UpdatedBy ?? d.CreatedBy
            });

            var model = new SearchResultModel
            {
                SearchModel = searchModel,
                SearchResults = documents
            };

            return View("Index", model);
        }

    }
}
