using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Models.Document;
using CertificateManagementSystem.Services.Components;
using Microsoft.AspNetCore.Http;
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

        public DocumentController(IDocumentService documents, IFileService files, IConfiguration configuration)
        {
            _configuration = configuration;
            _documents = documents;
            _files = files;
        }

        public IActionResult Create(DocumentType type)
        {
            CreateSelectLists();

            var model = new NewDocumentModel
            {
                ClientName="",
                ContractNumber="",
                DeviceName="",

                DocumentType = type,
                Year = 2020,
                CalibrationDate = DateTime.Now,
                CalibrationExpireDate = DateTime.Now.AddYears(1)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewDocumentModel model)
        {
            var newDocument = CreateDocument(model);

            // ПРОВЕРИТЬ ПРАВИЛЬНОСТЬ ВВЕДЕННЫХ ДАННЫХ:

            //var client = newDocument.Device.Contract.Client;
            //var regNumber = newDocument.Device.VerificationMethodic.RegistrationNumber;
            //var documentExist = _documents.IsDocumentExist(model.DocumentNumber);

            //// Валидация введенных данных
            //if (documentExist)
            //{
            //    // Документ с таким номером уже есть в базе
            //    ModelState.AddModelError("", "Документ с таким номером уже есть в базе.");
            //}
            //if (client.ExploitationPlace.ToLower() != model.ExploitationPlace.ToLower())
            //{
            //    // Место эксплуатации не совпадает
            //    ModelState.AddModelError("", "Место эксплуатации отличается от указанного ранее для этого договора.");
            //}
            //if (client.Name.ToLower() != model.ClientName.ToLower())
            //{
            //    // Имя заказчика не совпадает
            //    ModelState.AddModelError("", "Имя заказчика отличается от указанного ранее для этого договора.");
            //}
            //if (regNumber.ToLower() != model.RegistrationNumber)
            //{
            //    // Номер в гос.реестре не совпадает
            //    ModelState.AddModelError("", "За данным оборудованием закреплен другой номер в гос.реестре.");
            //}

            if (ModelState.IsValid)
            {
                //await _files.CreateFile(model.DocumentFile, newDocument.FilePath);
                await _documents.Add(newDocument);
            }

            CreateSelectLists();

            return View(model);
        }

        public JsonResult GetAutocompleteData(string dataType)
        {
            dynamic results = null;

            switch (dataType)
            {
                case "contract":
                    results = _documents.GetAllContracts()
                        .Select(c => new { id = c.ContractNumber, text = c.ContractNumber }).Distinct();
                    break;

                case "clientName":
                    results = _documents.GetAllClients()
                        .Select(c => new { id = c.Name, text = c.Name }).Distinct();
                    break;

                case "exploitationPlace":
                    results = _documents.GetAllClients()
                        .Select(c => new { id = c.ExploitationPlace, text = c.ExploitationPlace }).Distinct();
                    break;

                case "deviceName":
                    results = _documents.GetAllDevices()
                        .Select(d => new { id = d.Name, text = d.Name }).Distinct();
                    break;

                case "deviceType":
                    results = _documents.GetAllDevices()
                        .Select(d => new { id = d.Type, text = d.Type }).Distinct();
                    break;
                case "verificationMethodic":
                    break;

                default:
                    results = null;
                    break;
            }

            return Json(results);
        }

        public IActionResult SetClient(Contract contract)
        {
            var client = _documents.GetClient(contract);
            return Json(client);
        }

        //private string CreateFilePath(Document document)
        //{
        //    var isCertificate = document is Certificate;

        //    var year = document.Device.Contract.Year.ToString();
        //    var contract = document.Device.Contract.ContractNumber.ReplaceInvalidChars('-');
        //    var deviceType = document.Device.Type.ReplaceInvalidChars('-');
        //    var deviceName = document.Device.Name.ReplaceInvalidChars('-');
        //    var type = isCertificate ? "Свидетельства" : "Извещения о непригодности";

        //    var fileName = deviceType + "_" + deviceName;

        //    return Path.Combine(year, contract, type, fileName);
        //}

        private void CreateSelectLists()
        {
            var clients = _documents.GetAllClients();
            var devices = _documents.GetAllDevices();
            var methodics = _documents.GetAllVerificationMethodics();
            var contracts = _documents.GetAllContracts();

            var contractNumbers = contracts.Select(c => c.ContractNumber).Distinct();
            var clientNames = clients.Select(c => c.Name).Distinct();
            var exploitationPlaces = clients.OrderBy(c => c.ExploitationPlace).Select(c => c.ExploitationPlace).Distinct();
            var deviceNames = devices.Select(d => d.Name).Distinct();
            var deviceTypes = devices.OrderBy(d => d.Type).Select(d => d.Type).Distinct();
            var verificationMethodics = methodics.Select(vm => vm.Name).Distinct();
            var registerNumbers = methodics.OrderBy(vm => vm.RegistrationNumber).Select(vm => vm.RegistrationNumber).Distinct();

            ViewBag.Contracts = new SelectList(contracts);
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
            var contract = CreateContract(model);
            var methodic = CreateVerificationMethodic(model);

            if (device == null)
            {
                device = new Device
                {
                    Name = model.DeviceName?.Trim(),
                    Type = model.DeviceType?.Trim(),
                    SerialNumber = model.SerialNumber?.Trim(),
                    VerificationMethodic = methodic,
                    ContractDevices = new List<ContractDevice>
                    {
                        new ContractDevice
                        {
                            Contract = contract,
                            Device = device
                        }
                    }
                };
            }
            else
            {
                if (!device.ContractDevices.Any(cd => cd.Contract == contract))
                {
                    device.ContractDevices.Add(
                        new ContractDevice
                        {
                            Contract = contract,
                            Device = device
                        });
                }
            }

            return device;
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

            if (model.DocumentType == DocumentType.Certificate)
            {
                // Создаем новое свидетельство
                return new Certificate
                {
                    Device = device,
                    DocumentNumber = model.DocumentNumber?.Trim(),
                    CalibrationDate = model.CalibrationDate,
                    CalibrationExpireDate = model.CalibrationExpireDate
                };
            }
            else
            {
                // Создаем новое извещение о непригодности
                return new FailureNotification
                {
                    Device = device,
                    DocumentNumber = model.DocumentNumber?.Trim(),
                    DocumentDate = model.DocumentDate
                };
            }
        }
    }
}
