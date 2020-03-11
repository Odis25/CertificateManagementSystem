using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Models.Document;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Controllers
{
    public class DocumentController: Controller
    {
        private readonly IDocumentService _documents;

        public DocumentController(IDocumentService documents)
        {
            _documents = documents;
        }

        [HttpPost]
        public async Task<IActionResult> Create(NewDocumentModel model)
        {
            var contract = _documents.GetContract(model.ContractNumber);
            var device

            var document = new Certificate
            {
                RegistrationNumber = model.RegistrationNumber,
                CalibrationDate = model.CalibrationDate,
                CalibrationExpireDate = model.CalibrationExpireDate
            };

            await _documents.Add(document);

            return View();
        }
    }
}
