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

        [Required(ErrorMessage = "Нужно указать номер договора")]
        public string ContractNumber { get; set; }

        [Required(ErrorMessage = "Нужно указать номер документа")]
        public string DocumentNumber { get; set; }

        [Required(ErrorMessage = "Нужно указать название организации заказчика")]
        public string ClientName { get; set; }
        public string ExploitationPlace { get; set; }

        [Required(ErrorMessage = "Нужно указать название средства измерения")]
        public string DeviceName { get; set; }
        [Required(ErrorMessage = "Нужно указать тип средства измерения")]
        public string DeviceType { get; set; }
        [Required(ErrorMessage = "Нужно указать заводской номер средства измерения")]
        public string SerialNumber { get; set; }

        public string RegistrationNumber { get; set; }
        public string VerificationMethodic { get; set; }
        
        public DateTime CalibrationDate { get; set; }
        public DateTime CalibrationExpireDate { get; set; }
        public DateTime DocumentDate { get; set; }

        [Required(ErrorMessage = "Нужно выбрать файл документа")]
        public IFormFile DocumentFile { get; set; }
    }
}
