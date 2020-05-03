using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Extensions;
using CertificateManagementSystem.Models.Document;
using CertificateManagementSystem.Services.Components;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileProviders.Physical;
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

        public DocumentController(IDocumentService documents, IFileService files,
            IConfiguration configuration, IWebHostEnvironment appEnvironment)
        {
            _configuration = configuration;
            _documents = documents;
            _files = files;
            _appEnvironment = appEnvironment;
        }

        // Вывод на экран всех документов
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var years = _documents.GetYears();
            var yearModels = new List<YearModel>();

            foreach (var year in years)
            {
                var yearModel = new YearModel
                {
                    Year = year,
                    Contracts = _documents.GetContracts(year)
                    .Select(c => new ContractModel
                    {
                        Id = c.Id,
                        ContractNumber = c.ContractNumber
                    })
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
                ClientName = document.Contract.Client.Name,
                ExploitationPlace = document.Contract.Client.ExploitationPlace,
                DeviceName = document.Device.Name,
                DeviceType = document.Device.Type,
                SerialNumber = document.Device.SerialNumber,
                RegistrationNumber = document.Device.VerificationMethodic.RegistrationNumber,
                VerificationMethodic = document.Device.VerificationMethodic.Name,                
                DocumentNumber = document.DocumentNumber,
                DocumentType = (document is Certificate) ? "Свидетельство о поверке" : "Извещение о непригодности",
                CalibrationDate = (document as Certificate)?.CalibrationDate.ToString("dd.MM.yyyy"),
                CalibrationExpireDate = (document as Certificate)?.CalibrationExpireDate.ToString("dd.MM.yyyy"),
                DocumentDate = (document as FailureNotification)?.DocumentDate.ToString("dd.MM.yyyy"),
                FilePath = filePath
            };

            return PartialView("_DocumentPreview", model);
        }

        // Создание нового документа
        [HttpGet]
        public IActionResult Create(DocumentType type)
        {
            CreateSelectLists();

            var model = new NewDocumentModel
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
        public async Task<IActionResult> Create(NewDocumentModel model)
        {
            var newDocument = CreateDocument(model);

            if (ModelState.IsValid)
            {               
                var destinationFilePath = newDocument.DocumentFile.Path;
                //todo: реализовать запись в модель актуального пути к файлу
                try
                {
                    var sourceFilePath = UploadFile(model.DocumentFile);
                    _files.CreateFile(sourceFilePath, ref destinationFilePath);
                    await _documents.Add(newDocument);
                    this.AddAlertSuccess("Свидетельство успешно добавленно в базу!");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
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

        private string UploadFile(IFormFile documentFile)
        {
            var filePath = Path.Combine(_appEnvironment.WebRootPath, "files", documentFile.FileName);
            using(var file = new FileStream(filePath, FileMode.Create))
            {
                documentFile.CopyTo(file);
            }
            return filePath;
        }

        // Автоматическая подстановка заказчика при заполнении поля номера договора
        public IActionResult GetClient(Contract contract)
        {
            var client = _documents.GetClient(contract);
            return Json(client);
        }

        // Отобразить на странице документы соответствующие году и номеру договора
        public IActionResult LoadDocuments(int contractId)
        {
            var result = _documents.GetContractById(contractId).Documents?.Select(d =>
            new DocumentListingModel
            {
                Id = d.Id,
                Year = d.Contract.Year,
                ContractId = d.Contract.Id,
                ContractNumber = d.Contract.ContractNumber,
                ClientId = d.Contract.Client.Id,
                ClientName = d.Contract.Client.Name,
                ExploitationPlace = d.Contract.Client.ExploitationPlace,
                DeviceId = d.Device.Id,
                DeviceType = d.Device.Type,
                DeviceName = d.Device.Name,
                SerialNumber = d.Device.SerialNumber,
                RegistrationNumber = d.Device.VerificationMethodic?.RegistrationNumber,
                VerificationMethodic = d.Device.VerificationMethodic?.Name,
                DocumentNumber = d.DocumentNumber,
                CalibrationDate = (d as Certificate)?.CalibrationDate.ToString("dd.MM.yyyy"),
                CalibrationExpireDate = (d as Certificate)?.CalibrationExpireDate.ToString("dd.MM.yyyy"),
                FilePath = d.DocumentFile.Path,
                DocumentType = (d is Certificate) ? "Свидетельство" : "Извещение о непригодности",
                DocumentDate = (d as FailureNotification)?.DocumentDate.ToString("dd.MM.yyyy")
            });

            var model = new DocumentIndexModel { Documents = result };

            return PartialView("_DocumentsTable", model);
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
        private Client CreateClient(NewDocumentModel model)
        {
            return _documents.GetClient(model.ClientName, model.ExploitationPlace) ??
                new Client
                {
                    Name = model.ClientName?.Trim().Capitalize(),
                    ExploitationPlace = model.ExploitationPlace?.Trim().Capitalize()
                };
        }

        // Формируем новый договор
        private Contract CreateContract(NewDocumentModel model)
        {
            var contract = _documents.GetContract(model.ContractNumber, model.Year);

            if (contract == null)
            {
                contract = new Contract
                {
                    Year = model.Year,
                    ContractNumber = model.ContractNumber?.Trim(),
                    Client = CreateClient(model)
                };
            }
            else
            {
                if (contract.Client.Name.ToLower() != model.ClientName.ToLower())
                {
                    // Имя заказчика не совпадает
                    ModelState.AddModelError("", "Имя заказчика отличается от указанного ранее для этого договора.");
                }
                if (contract.Client.ExploitationPlace?.ToLower() != model.ExploitationPlace?.ToLower())
                {
                    // Место эксплуатации не совпадает
                    ModelState.AddModelError("", "Место эксплуатации отличается от указанного ранее для этого договора.");
                }
            }

            return contract;
        }

        // Формируем новую метоздику поверки
        private VerificationMethodic CreateVerificationMethodic(NewDocumentModel model)
        {
            var methodic = _documents.GetVerificationMethodic(model.RegistrationNumber);
            if (methodic == null)
            {
                if (model.VerificationMethodic != null)
                {
                    methodic = new VerificationMethodic
                    {
                        Name = model.VerificationMethodic,
                        RegistrationNumber = model.RegistrationNumber?.Trim(),
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
        private Device CreateDevice(NewDocumentModel model)
        {
            var device = _documents.GetDevice(model.DeviceName, model.SerialNumber);
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
        private FileModel CreateFilePath(NewDocumentModel model)
        {
            var year = model.Year.ToString();
            var contract = model.ContractNumber.ReplaceInvalidChars('-') ?? "";
            var deviceType = model.DeviceType.ReplaceInvalidChars('-');
            var deviceName = model.DeviceName.ReplaceInvalidChars('-');
            var type = model.DocumentType == DocumentType.Certificate ? "Свидетельства" : "Извещения о непригодности";

            var extension = Path.GetExtension(model.DocumentFile?.FileName);
            var fileName = deviceType + "_" + deviceName + extension;
            var filePath = Path.Combine(year, contract, type, fileName);

            var file = new FileModel
            {
                Size = model.DocumentFile?.Length ?? 0,
                ContentType = model.DocumentFile?.ContentType,
                Path = filePath
            };

            return file;
        }

        // Формируем новый документ
        private Document CreateDocument(NewDocumentModel model)
        {
            if (_documents.IsDocumentExist(model.DocumentNumber))
            {
                // Документ с таким номером уже есть в базе
                ModelState.AddModelError("", "Документ с таким номером уже есть в базе.");
            }

            var device = CreateDevice(model);
            var contract = CreateContract(model);

            if (model.DocumentType == DocumentType.Certificate)
            {
                // Создаем новое свидетельство
                return new Certificate
                {
                    Device = device,
                    Contract = contract,
                    DocumentNumber = model.DocumentNumber?.Trim(),
                    CalibrationDate = model.CalibrationDate,
                    CalibrationExpireDate = model.CalibrationExpireDate,
                    DocumentFile = CreateFilePath(model)
                };
            }
            else
            {
                // Создаем новое извещение о непригодности
                return new FailureNotification
                {
                    Device = device,
                    Contract = contract,
                    DocumentNumber = model.DocumentNumber?.Trim(),
                    DocumentDate = model.DocumentDate,
                    DocumentFile = CreateFilePath(model)
                };
            }
        }
    }
}
