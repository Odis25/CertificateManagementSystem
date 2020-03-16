using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Models.Document;
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

        public IActionResult Create()
        {
            var model = new NewDocumentModel
            {
                DocumentNumber = "TestDocumentNumber",
                ClientName = "TestClient",
                ExploitationPlace = "TestPlace",
                ContractNumber = "TestContractNumber",
                DeviceName = "TestDeviceName",
                DeviceType = "TestDeviceType",
                RegistrationNumber = "TestRegNumber",
                SerialNumber = "TestSerialNumber",
                Year = 2020,
                CalibrationDate = DateTime.Now,
                CalibrationExpireDate = DateTime.Now.AddDays(30)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewDocumentModel model)
        {
            // Проверка правильности введенных данных
            if (ModelState.IsValid)
            {
                var newDocument = BuildNewDocument(model);

                // Проверка корректности введенных данных
                if (newDocument.Device.Contract.Client.ExploitationPlace != model.ExploitationPlace)
                {
                    // Место эксплуатации не совпадает
                    ModelState.AddModelError("", "Место эксплуатации отличается от указанного ранее для этого номера договора");
                }
                if (newDocument.Device.Contract.Client.Name != model.ClientName)
                {
                    // Имя заказчика не совпадает
                    ModelState.AddModelError("", "Имя заказчика отличается от указанного ранее");
                }
                if (newDocument.Device.VerificationMethodic.RegistrationNumber != model.RegistrationNumber)
                {
                    // Номер в гос.реестре не совпадает
                    ModelState.AddModelError("", "За данным оборудованием закреплен другой номер в гос.реестре.");
                }

                var result = await _documents.Add(newDocument);

                if (result == 1)
                {
                    ModelState.AddModelError("", "Свидетельство с таким номером уже есть в базе.");
                }
            }

            return View(model);
        }

        private Document BuildNewDocument(NewDocumentModel model)
        {
            // Находим существующее средство измерения, или создаем новое
            var device = _documents.GetDevice(model.DeviceName, model.SerialNumber, model.ContractNumber, model.Year);

            device ??= new Device
            {
                Name = model.DeviceName,
                Type = model.DeviceType,
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
                        Name = model.ClientName,
                        ExploitationPlace = model.ExploitationPlace
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
           
            // Создаем новое свидетельство
            return new Certificate
            {
                Device = device,
                DocumentNumber = model.DocumentNumber,
                CalibrationDate = model.CalibrationDate,
                CalibrationExpireDate = model.CalibrationExpireDate
            };
        }
    }
}
