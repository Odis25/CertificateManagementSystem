using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Models.Document;
using CertificateManagementSystem.Services.Components;
using Microsoft.AspNetCore.Mvc;
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

            return View(model);
        }


        public JsonResult GetClient(string contractNumber, int year)
        {
            var contract = new Contract
            {
                ContractNumber = contractNumber,
                Year = year
            };
            var result = _documents.GetClient(contract);
            var data = Json(result);

            return data;
        }

        public JsonResult GetAutocompleteData(string dataType)
        {
            dynamic result = null;

            switch (dataType)
            {
                case "contract":
                    result = _documents.GetAllContracts()
                        .Select(c => new { id = c.Id, text = c.ContractNumber }).Distinct();
                    break;

                case "clientName":
                    result = _documents.GetAllClients()
                        .Select(c => new { id = c.Id, text = c.Name }).Distinct();
                    break;

                case "exploitationPlace":
                    result = _documents.GetAllClients()
                        .Select(c => new { id = c.Id, text = c.ExploitationPlace }).Distinct();
                    break;

                case "deviceName":
                    result = _documents.GetAllDevices()
                        .Select(d => new { id = d.Id, text = d.Name }).Distinct();
                    break;

                case "deviceType":
                    result = _documents.GetAllDevices()
                        .Select(d => new { id = d.Id, text = d.Type }).Distinct();
                    break;
                case "verificationMethodic":
                    break;

                default:
                    result = null;
                    break;
            }

            return Json(result);
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
    }
}
