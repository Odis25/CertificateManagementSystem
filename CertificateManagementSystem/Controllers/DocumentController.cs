﻿using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Extensions;
using CertificateManagementSystem.Models.Document;
using CertificateManagementSystem.Services.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IDocumentService _documents;
        private readonly IFileService _files;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _appEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public DocumentController(IDocumentService documents, IFileService files,
            IConfiguration configuration, IWebHostEnvironment appEnvironment, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _documents = documents;
            _files = files;
            _appEnvironment = appEnvironment;
            _userManager = userManager;
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

        // Детализированная информация о документе
        [HttpGet]
        public IActionResult Details(int id)
        {
            var document = _documents.GetDocumentById(id);

            var filePath = Path.Combine("/fileserver", document.DocumentFile.Path);

            var model = new DocumentListingModel
            {
                Year = document.Contract.Year,
                ContractNumber = document.Contract.ContractNumber,
                ClientName = document.Client.Name,
                ExploitationPlace = document.Client.ExploitationPlace,
                DeviceName = document.Device.Name,
                DeviceType = document.Device.Type,
                SerialNumber = document.Device.SerialNumber,
                RegistrationNumber = document.Device.VerificationMethodic?.RegistrationNumber,
                VerificationMethodic = document.Device.VerificationMethodic?.Name,
                DocumentNumber = document.DocumentNumber,
                DocumentType = (document is Certificate) ? "Свидетельство о поверке" : "Извещение о непригодности",
                CalibrationDate = (document as Certificate)?.CalibrationDate.ToString("dd-MM-yyyy"),
                CalibrationExpireDate = (document as Certificate)?.CalibrationExpireDate.ToString("dd-MM-yyyy"),
                DocumentDate = (document as FailureNotification)?.DocumentDate.ToString("dd-MM-yyyy"),
                CreatedOn = document.CreatedOn.ToString("dd-MM-yyyy hh:mm"),
                UpdatedOn = document.UpdatedOn?.ToString("dd-MM-yyyy hh:mm") ?? document.CreatedOn.ToString("dd-MM-yyyy hh:mm"),
                CreatedBy = document.CreatedBy,
                UpdatedBy = document.UpdatedBy ?? document.CreatedBy,
                FilePath = filePath
            };

            return PartialView("_DocumentPreview", model);
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
         
            var model = new CreateDocumentModel
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
        public async Task<IActionResult> Create(CreateDocumentModel model)
        {
            var newDocument = CreateDocument(model);

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var destinationFilePath = newDocument.DocumentFile.Path;

                //todo: не забыть раскоментировать
                //newDocument.CreatedBy = user.FullName;
                newDocument.CreatedOn = DateTime.Now;

                try
                {
                    // Загружаем файл на сервер
                    var sourceFilePath = UploadFile(model.DocumentFile);
                    // Создаем файл по месту хранения
                    _files.CreateFile(sourceFilePath, destinationFilePath);
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
            var methodics = _documents.GetVerificationMethodics();
            var contracts = _documents.GetContracts();

            var contractNumbers = contracts.Select(c => c.ContractNumber).Distinct();
            var clientNames = clients.Select(c => c.Name).Distinct();
            var exploitationPlaces = clients.OrderBy(c => c.ExploitationPlace).Select(c => c.ExploitationPlace).Distinct();
            var deviceNames = devices.Select(d => d.Name).Distinct();
            var deviceTypes = devices.OrderBy(d => d.Type).Select(d => d.Type).Distinct();
            var verificationMethodics = methodics.Select(vm => vm.Name).Distinct();
            var registerNumbers = methodics.OrderBy(vm => vm.RegistrationNumber).Select(vm => vm.RegistrationNumber).Distinct();

            ViewBag.Contracts = new SelectList(contractNumbers);
            ViewBag.ClientNames = new SelectList(clientNames);
            ViewBag.ExploitationPlaces = new SelectList(exploitationPlaces);
            ViewBag.DeviceNames = new SelectList(deviceNames);
            ViewBag.DeviceTypes = new SelectList(deviceTypes);
            ViewBag.VerificationMethodics = new SelectList(verificationMethodics);
            ViewBag.RegisterNumbers = new SelectList(registerNumbers);
        }

        // Формируем нового заказчика
        private Client CreateClient(CreateDocumentModel model)
        {
            return _documents.FindClient(model.ClientName, model.ExploitationPlace) ??
                new Client
                {
                    Name = model.ClientName?.Trim().Capitalize(),
                    ExploitationPlace = model.ExploitationPlace?.Trim().Capitalize()
                };
        }

        // Формируем новый договор
        private Contract CreateContract(CreateDocumentModel model)
        {
            return _documents.FindContract(model.ContractNumber, model.Year)
                ?? new Contract
                {
                    Year = model.Year,
                    ContractNumber = model.ContractNumber?.Trim()
                };
        }

        // Формируем новую метоздику поверки
        private VerificationMethodic CreateVerificationMethodic(CreateDocumentModel model)
        {
            var methodic = _documents.FindVerificationMethodic(model.RegistrationNumber);
            if (methodic == null)
            {
                if (model.VerificationMethodic != null)
                {
                    methodic = new VerificationMethodic
                    {
                        Name = model.VerificationMethodic,
                        RegistrationNumber = model.RegistrationNumber?.Trim(),
                        //todo: продумать как добавлять методику поверки
                        FileName = ""
                    };
                }
            }
            else
            {
                //todo: Что если методика поверки и регистрационный номер отличаются от указанных ранее для этого устройства?
                if (methodic.RegistrationNumber.ToLower() != model.RegistrationNumber?.ToLower())
                {
                    // Номер в гос.реестре не совпадает
                    ModelState.AddModelError("", "За данным оборудованием закреплен другой номер в гос.реестре.");
                }
                if (methodic.Name.ToLower() != model.VerificationMethodic?.ToLower())
                {
                    // Название методики поверки не совпадает
                    ModelState.AddModelError("", "За данным оборудованием закреплена другая методика поверки.");
                }
            }

            return methodic;
        }

        // Формируем новое устройство
        private Device CreateDevice(CreateDocumentModel model)
        {
            var device = _documents.FindDevice(model.DeviceName, model.SerialNumber);
            var methodic = CreateVerificationMethodic(model);

            device ??= new Device
            {
                Name = model.DeviceName?.Trim(),
                Type = model.DeviceType?.Trim(),
                SerialNumber = model.SerialNumber?.Trim(),
                VerificationMethodic = methodic
            };
            return device;
        }

        // Формируем модель файла документа
        private FileModel CreateFilePath(CreateDocumentModel model)
        {
            var year = model.Year.ToString();
            var contract = model.ContractNumber.ReplaceInvalidChars('-') ?? "";
            var deviceType = model.DeviceType.ReplaceInvalidChars('-');
            var deviceName = model.DeviceName.ReplaceInvalidChars('-');
            var docType = model.DocumentType == DocumentType.Certificate ? "Свидетельства" : "Извещения о непригодности";

            var extension = Path.GetExtension(model.DocumentFile?.FileName);
            var fileName = deviceType + "_" + deviceName + extension;
            var filePath = Path.Combine(year, contract, docType, fileName);

            var file = new FileModel
            {
                Size = model.DocumentFile?.Length ?? 0,
                ContentType = model.DocumentFile?.ContentType,
                Path = filePath
            };
            // Актуализируем путь к файлу
            file.Path = _files.GetRealFilePath(file);

            return file;
        }

        // Формируем новый документ
        private Document CreateDocument(CreateDocumentModel model)
        {
            // Если документ с таким номером уже есть в базе
            if (_documents.IsDocumentExist(model.DocumentNumber))
            {
                ModelState.AddModelError("", "Документ с таким номером уже есть в базе.");
                return null;
            }

            var device = CreateDevice(model);
            var contract = CreateContract(model);
            var client = CreateClient(model);
            var filePath = CreateFilePath(model);

            switch (model.DocumentType)
            {
                // Создаем новое свидетельство
                case DocumentType.Certificate:
                    return new Certificate
                    {
                        Device = device,
                        Contract = contract,
                        Client = client,
                        DocumentNumber = model.DocumentNumber?.Trim(),
                        CalibrationDate = model.CalibrationDate,
                        CalibrationExpireDate = model.CalibrationExpireDate,
                        DocumentFile = filePath
                    };

                // Создаем новое извещение о непригодности
                case DocumentType.FailureNotification:
                    return new FailureNotification
                    {
                        Device = device,
                        Contract = contract,
                        Client = client,
                        DocumentNumber = model.DocumentNumber?.Trim(),
                        DocumentDate = model.DocumentDate,
                        DocumentFile = filePath
                    };

                default:
                    return null;
            }
        }
        
        // Загрузка файла на сервер
        private string UploadFile(IFormFile documentFile)
        {
            var fileName = User.Identity.Name + "_temp.tmp";
            var filePath = Path.Combine(_appEnvironment.WebRootPath, "files", "uploads", fileName);

            using (var file = new FileStream(filePath, FileMode.Create))
            {
                documentFile.CopyTo(file);
            }

            return filePath;
        }
    }
}
