using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Models.Document;
using CertificateManagementSystem.Services.Components;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Controllers
{
    public class DocumentController : Controller
    {
        private readonly IDocumentService _documents;

        public DocumentController(IDocumentService documents)
        {
            _documents = documents;
        }

        public IActionResult Create(DocumentType type)
        {
            CreateSelectLists();

            var model = new NewDocumentModel
            {
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
            var newDocument = BuildNewDocument(model);
            var client = newDocument.Device.Contract.Client;
            var regNumber = newDocument.Device.VerificationMethodic.RegistrationNumber;
            var documentExist = _documents.IsDocumentExist(model.DocumentNumber);

            // Проверка корректности введенных данных
            if (client.ExploitationPlace != model.ExploitationPlace)
            {
                // Место эксплуатации не совпадает
                ModelState.AddModelError("", "Место эксплуатации отличается от указанного ранее для этого договора.");
            }
            if (client.Name != model.ClientName)
            {
                // Имя заказчика не совпадает
                ModelState.AddModelError("", "Имя заказчика отличается от указанного ранее для этого договора.");
            }
            if (regNumber != model.RegistrationNumber)
            {
                // Номер в гос.реестре не совпадает
                ModelState.AddModelError("", "За данным оборудованием закреплен другой номер в гос.реестре.");
            }

            if (documentExist)
            {
                // Документ с таким номером уже есть в базе
                ModelState.AddModelError("", "Документ с таким номером уже есть в базе.");
            }

            // Проверка правильности введенных данных
            if (ModelState.IsValid)
            {
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

        private Document BuildNewDocument(NewDocumentModel model)
        {
            // Находим существующее средство измерения, или создаем новое
            var device = _documents.GetDevice(model.DeviceName, model.SerialNumber, model.ContractNumber, model.Year);

            device ??= new Device
            {
                Name = model.DeviceName.Capitalize(),
                Type = model.DeviceType.Capitalize(),
                SerialNumber = model.SerialNumber,
                // Находим существующий договор, или создаем новый
                Contract = _documents.GetContract(model.ContractNumber, model.Year) ??
                new Contract
                {
                    Year = model.Year,
                    ContractNumber = model.ContractNumber,
                    // Находим существующего заказчика, или создаем нового
                    Client = _documents.GetClient(model.ClientName, model.ExploitationPlace) ??
                    new Client
                    {
                        Name = model.ClientName.Capitalize(),
                        ExploitationPlace = model.ExploitationPlace.Capitalize()
                    }
                },
                // Находим существующую методику, или создаем новую
                VerificationMethodic = _documents.GetVerificationMethodic(model.RegistrationNumber) ??
                new VerificationMethodic
                {
                    RegistrationNumber = model.RegistrationNumber,
                    Name = model.VerificationMethodic,
                    FileName = ""
                    // todo: продумать логику добавления методики поверки
                }
            };

            if (model.DocumentType == DocumentType.Certificate)
            {
                // Создаем новое свидетельство
                return new Certificate
                {
                    Device = device,
                    DocumentNumber = model.DocumentNumber,
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
                    DocumentNumber = model.DocumentNumber,
                    DocumentDate = model.DocumentDate
                };
            }

        }
        private void CreateSelectLists()
        {
            var clients = _documents.GetAllClients();
            var devices = _documents.GetAllDevices();

            var clientNames = clients.OrderBy(c => c.Name).Select(c => c.Name).Distinct();
            var exploitationPlaces = clients.OrderBy(c => c.ExploitationPlace).Select(c => c.ExploitationPlace).Distinct();
            var deviceNames = devices.OrderBy(d => d.Name).Select(d => d.Name).Distinct();
            var deviceTypes = devices.OrderBy(d => d.Type).Select(d => d.Type).Distinct();
            var verificationMethodics = devices.OrderBy(d => d.VerificationMethodic).Select(d => d.VerificationMethodic);

            ViewBag.ClientNames = new SelectList(clientNames);
            ViewBag.ExploitationPlaces = new SelectList(exploitationPlaces);
            ViewBag.DeviceNames = new SelectList(deviceNames);
            ViewBag.DeviceTypes = new SelectList(deviceTypes);
            ViewBag.VerificationMethodics = new SelectList(verificationMethodics);
        }
    }
}
