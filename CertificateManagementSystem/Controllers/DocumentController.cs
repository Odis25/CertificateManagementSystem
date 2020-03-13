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
                var device = _documents.GetDevice(model.DeviceName, model.SerialNumber);
                var contract = _documents.GetContract(model.ContractNumber, model.Year);
                var client = _documents.GetClient(model.ClientName, model.ExploitationPlace);
                var verificationMethodic = _documents.GetVerificationMethodic(model.RegistrationNumber);

                if (device == null)
                {
                    device = new Device
                    {
                        Name = model.DeviceName,
                        Type = model.DeviceType,
                        SerialNumber = model.SerialNumber,
                        Contract = contract ?? new Contract
                        {
                            Year = model.Year,
                            ContractNumber = model.ContractNumber,
                            Client = client ?? new Client
                            {
                                Name = model.ClientName,
                                ExploitationPlace = model.ExploitationPlace
                            }
                        },
                        VerificationMethodic = verificationMethodic ?? new VerificationMethodic
                        {
                            RegistrationNumber = model.RegistrationNumber,
                            Name = model.VerificationMethodic,
                            FileName = ""
                            // todo: продумать логику добавления методики поверки
                        }
                    };
                }

                var document = new Certificate
                {
                    Device = device,
                    DocumentNumber = model.DocumentNumber,
                    CalibrationDate = model.CalibrationDate,
                    CalibrationExpireDate = model.CalibrationExpireDate
                };

                await _documents.Add(document);
            }

            return View(model);
        }
    }
}
