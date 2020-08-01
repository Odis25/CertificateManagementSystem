using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Extensions;
using CertificateManagementSystem.Helpers;
using CertificateManagementSystem.Models.Document;
using CertificateManagementSystem.Services.Components;
using CertificateManagementSystem.Services.Interfaces;
using CertificateManagementSystem.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IDocumentService _documents;
        private readonly IFileProvider _fileProvider;
        private readonly IFileService _files;
        private readonly IViewModelMapper _vmMapper;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public DocumentController(IDocumentService documents,
            IConfiguration configuration,
            IFileProvider fileProvider,
            IFileService files,
            IViewModelMapper vmMapper,
            IWebHostEnvironment appEnvironment,
            UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _documents = documents;
            _files = files;
            _appEnvironment = appEnvironment;
            _userManager = userManager;
            _fileProvider = fileProvider;
            _vmMapper = vmMapper;
        }

        // Построение дерева навигации по документам
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var years = _documents.GetYears();
            var yearModels = new List<YearModel>();

            foreach (var year in years)
            {
                var contractModels = _documents.GetContracts(year)
                    .Select(c => new ContractModel
                    {
                        Id = c.Id,
                        ContractNumber = c.ContractNumber
                    });

                var yearModel = new YearModel
                {
                    Year = year,
                    Contracts = contractModels
                };

                yearModels.Add(yearModel);
            }

            var model = new DocumentIndexModel
            {
                DocumentsCount = await _documents.DocumentsCount(),
                CertificatesCount = await _documents.CertificatesCount(),
                FailureNotificationsCount = await _documents.FailureNotificationsCount(),
                Years = yearModels
            };

            return View(model);
        }

        // Подробная информация о документе
        [HttpGet]
        public IActionResult Details(int id)
        {
            var document = _documents.GetDocumentById(id);

            var filePath = Path.Combine("/documentsFolder", document.DocumentFile.Path);

            var model = new DocumentDetailsModel
            {
                Id = document.Id,
                Year = document.Contract.Year,
                ContractId = document.Contract.Id,
                ContractNumber = document.Contract.ContractNumber,
                ClientId = document.Client.Id,
                ClientName = document.Client.Name,
                ExploitationPlace = document.Client.ExploitationPlace,
                DeviceId = document.Device.Id,
                DeviceName = document.Device.Name,
                DeviceType = document.Device.Type,
                SerialNumber = document.Device.SerialNumber,
                RegistrationNumber = document.Device.RegistrationNumber,
                VerificationMethodic = document.Device.VerificationMethodic?.Name,
                DocumentNumber = document.DocumentNumber,
                DocumentType = (document is CertificateDTO) ? "Свидетельство о поверке" : "Извещение о непригодности",
                CalibrationDate = (document as CertificateDTO)?.CalibrationDate.ToString("dd-MM-yyyy"),
                CalibrationExpireDate = (document as CertificateDTO)?.CalibrationExpireDate.ToString("dd-MM-yyyy"),
                DocumentDate = (document as FailureNotificationDTO)?.DocumentDate.ToString("dd-MM-yyyy"),
                CreatedOn = document.CreatedOn.ToString("dd-MM-yyyy HH:mm"),
                UpdatedOn = document.UpdatedOn?.ToString("dd-MM-yyyy HH:mm") ?? document.CreatedOn.ToString("dd-MM-yyyy HH:mm"),
                CreatedBy = document.CreatedBy,
                UpdatedBy = document.UpdatedBy ?? document.CreatedBy,
                FilePath = filePath
            };

            return PartialView("_DocumentDetails", model);
        }

        // Редактирование документа
        [HttpGet]
        [Authorize(Roles = "Admin, Metrologist")]
        public IActionResult DocumentEdit(int id)
        {
            var document = _documents.GetDocumentById(id);

            var filePath = Path.Combine("/documentsFolder", document.DocumentFile.Path);
            var model = new DocumentEditModel
            {
                Id = document.Id,
                Year = document.Contract.Year,
                ContractNumber = document.Contract.ContractNumber,
                ClientName = document.Client.Name,
                ExploitationPlace = document.Client.ExploitationPlace,
                DeviceName = document.Device.Name,
                DeviceType = document.Device.Type,
                SerialNumber = document.Device.SerialNumber,
                RegistrationNumber = document.Device.RegistrationNumber,
                VerificationMethodic = document.Device.VerificationMethodic?.FileName,
                DocumentNumber = document.DocumentNumber,
                DocumentType = (document is CertificateDTO) ? DocumentType.Certificate : DocumentType.FailureNotification,
                CalibrationDate = (document as CertificateDTO)?.CalibrationDate,
                CalibrationExpireDate = (document as CertificateDTO)?.CalibrationExpireDate,
                DocumentDate = (document as FailureNotificationDTO)?.DocumentDate,
                FilePath = filePath
            };

            CreateSelectLists();

            return PartialView("_DocumentEdit", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Metrologist")]
        public async Task<IActionResult> DocumentEdit(DocumentEditModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                model.UpdatedOn = DateTime.Now;
                model.UpdatedBy = user.FullName;

                var document = _vmMapper.MapDocumentModel(model);

                await _documents.Edit(document);
            }

            CreateSelectLists();

            // Отображаем ошибки операции
            foreach (var state in ModelState.Values)
            {
                foreach (var error in state.Errors)
                {
                    this.AddAlertDanger(error.ErrorMessage);
                }
            }

            return PartialView("_DocumentEdit", model);
        }

        // Отобразить на странице документы соответствующие году и номеру договора
        [HttpGet]
        public IActionResult LoadDocuments(int contractId)
        {
            var result = _documents.GetDocumentsByContractId(contractId)?.Select(d =>
            new DocumentListingModel
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
                RegistrationNumber = d.Device.RegistrationNumber,
                VerificationMethodic = d.Device.VerificationMethodic?.Name,
                DocumentNumber = d.DocumentNumber,
                CalibrationDate = (d as CertificateDTO)?.CalibrationDate.ToString("dd-MM-yyyy"),
                CalibrationExpireDate = (d as CertificateDTO)?.CalibrationExpireDate.ToString("dd-MM-yyyy"),
                FilePath = d.DocumentFile.Path,
                DocumentType = (d is CertificateDTO) ? "Свидетельство" : "Извещение о непригодности",
                DocumentDate = (d as FailureNotificationDTO)?.DocumentDate.ToString("dd-MM-yyyy"),
                CreatedOn = d.CreatedOn.ToString("dd-MM-yyyy hh:mm"),
                UpdatedOn = d.UpdatedOn?.ToString("dd-MM-yyyy hh:mm") ?? d.CreatedOn.ToString("dd-MM-yyyy hh:mm"),
                CreatedBy = d.CreatedBy,
                UpdatedBy = d.UpdatedBy ?? d.CreatedBy,
            });

            var model = new DocumentIndexModel { Documents = result };

            return PartialView("_DocumentsTable", model);
        }

        // Создание нового документа
        [HttpGet]
        [Authorize(Roles = "Admin, Metrologist")]
        public IActionResult Create(DocumentType type)
        {
            CreateSelectLists();

            var model = new DocumentCreateModel
            {
                DocumentType = type,
                Year = DateTime.Now.Year,
                CalibrationDate = DateTime.Now,
                CalibrationExpireDate = DateTime.Now.AddYears(1),
                DocumentDate = DateTime.Now
            };

            return View(model);
        }

        // Создание нового документа
        [HttpPost]
        [Authorize(Roles = "Admin, Metrologist")]
        public async Task<IActionResult> Create(DocumentCreateModel model)
        {
            //todo: создать отдельный класс валидации свойства
            // Если документ с таким номером уже есть в базе
            if (_documents.IsDocumentExist(model.DocumentNumber))
            {
                ModelState.AddModelError("", "Документ с таким номером уже есть в базе.");
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                model.CreatedBy = user.FullName;
                model.CreatedOn = DateTime.Now;

                // Маппинг сущности
                var newDocument = _vmMapper.MapDocumentModel(model);
                // Путь к создаваемому файлу
                var destination = newDocument.DocumentFile.Path;

                try
                {
                    // Загружаем файл на сервер
                    var result = _files.UploadFile(model.DocumentFile, destination);                   
                    // Добавляем запись в базу
                    await _documents.Add(newDocument);
                    // Уведомляем пользователя об успешном добавлении
                    this.AddAlertSuccess("Свидетельство успешно добавленно в базу!");
                }
                catch (Exception exception)
                {
                    ModelState.AddModelError("", exception.Message);
                }
            }

            CreateSelectLists();

            // Отображаем ошибки операции
            foreach (var state in ModelState.Values)
            {
                foreach (var error in state.Errors)
                {
                    this.AddAlertDanger(error.ErrorMessage);
                }
            }

            return View(model);
        }

        // Формируем списки автозаполнения 
        private void CreateSelectLists()
        {
            var clients = _documents.GetClients();
            var devices = _documents.GetDevices();
            var contracts = _documents.GetContracts();
            var methodics = _documents.GetMethodics();

            var contractNumbers = contracts.Select(c => c.ContractNumber).Distinct();
            var clientNames = clients.Select(c => c.Name).Distinct();
            var exploitationPlaces = clients.OrderBy(c => c.ExploitationPlace).Select(c => c.ExploitationPlace).Distinct();
            var deviceNames = devices.Select(d => d.Name).Distinct();
            var deviceTypes = devices.OrderBy(d => d.Type).Select(d => d.Type).Distinct();
            var registerNumbers = devices.OrderBy(d => d.RegistrationNumber).Select(d => d.RegistrationNumber).Distinct();

            ViewBag.Contracts = new SelectList(contractNumbers);
            ViewBag.ClientNames = new SelectList(clientNames);
            ViewBag.DeviceNames = new SelectList(deviceNames);
            ViewBag.DeviceTypes = new SelectList(deviceTypes);
            ViewBag.ExploitationPlaces = new SelectList(exploitationPlaces);
            ViewBag.RegisterNumbers = new SelectList(registerNumbers);
            ViewBag.Methodics = new SelectList(methodics, "FileName", "Name");
        }
    }
}
