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

        public async Task<IActionResult> Create()
        {
            var model = new NewDocumentModel
            {
                ClientName = "TestClient",
                ExploitationPlace = "TestPlace1",
                ContractNumber = "TestContractNumber",
                DeviceName = "TestDeviceName",
                DeviceType = "TestDeviceType",
                RegistrationNumber = "TestRegNumber",
                SerialNumber = "TestSerialNumber",
                Year = 2020,
                CalibrationDate = DateTime.Now,
                CalibrationExpireDate = DateTime.Now.AddDays(30)
            };

            var client = _documents.GetClient(model.ClientName);

            if (client == null)
            {
                client = new Client
                {
                    Name = model.ClientName
                };
                var expPlace = new ExploitationPlace
                {
                    Name = model.ExploitationPlace,
                    Client = client
                };
            }
            else
            {
                if (!client.ExploitationPlaces.Any(ep => ep.Name == model.ExploitationPlace))
                {
                    var expPlace = new ExploitationPlace
                    {
                        Name = model.ExploitationPlace,
                        Client = client
                    };
                }
            }

            var contract = _documents.GetContract(model.ContractNumber);

            contract ??= new Contract
            {
                Year = model.Year,
                ContractNumber = model.ContractNumber,
                Client = client
            };

            var device = _documents.GetDevice(model.DeviceName, model.SerialNumber);

            device ??= new Device
            {
                Name = model.DeviceName,
                Type = model.DeviceType,
                SerialNumber = model.SerialNumber,
                Contract = contract
            };

            var document = new Certificate
            {
                Device = device,
                RegistrationNumber = model.RegistrationNumber,
                CalibrationDate = model.CalibrationDate,
                CalibrationExpireDate = model.CalibrationExpireDate
            };

            await _documents.Add(document);

            return View("~/Home/Index");
        }
    }
}
