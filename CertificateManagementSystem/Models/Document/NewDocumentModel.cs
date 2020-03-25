using CertificateManagementSystem.Services.Components;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace CertificateManagementSystem.Models.Document
{
    public class NewDocumentModel
    {
        public int Year { get; set; }

        public DocumentType DocumentType { get; set; }

        [Required(ErrorMessage = "Укажите номер договора")]
        public string ContractNumber { get; set; }

        [Required(ErrorMessage = "Укажите номер документа")]
        public string DocumentNumber { get; set; }

        [Required(ErrorMessage = "Укажите название организации заказчика")]
        public string ClientName { get; set; }
        public string ExploitationPlace { get; set; }

        [Required(ErrorMessage = "Укажите название средства измерения")]
        public string DeviceName { get; set; }
        [Required(ErrorMessage = "Укажите тип средства измерения")]
        public string DeviceType { get; set; }
        [Required(ErrorMessage = "Укажите заводской номер средства измерения")]
        public string SerialNumber { get; set; }

        public string RegistrationNumber { get; set; }
        public string VerificationMethodic { get; set; }
        
        public DateTime CalibrationDate { get; set; }
        public DateTime CalibrationExpireDate { get; set; }
        public DateTime DocumentDate { get; set; }

        public IFormFile DocumentFile { get; set; }
    }
}
