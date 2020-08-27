using CertificateManagementSystem.Services.Components;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;

namespace CertificateManagementSystem.Services.ViewModels.Document
{
    public class DocumentCreateModel
    {
        public int Year { get; set; }

        public DocumentType DocumentType { get; set; }

        [Required(ErrorMessage = "Не указан номер договора")]
        public string ContractNumber { get; set; }

        [Required(ErrorMessage = "Не указан номер документа")]
        public string DocumentNumber { get; set; }

        [Required(ErrorMessage = "Не указано название организации заказчика")]
        public string ClientName { get; set; }
        public string ExploitationPlace { get; set; }

        [Required(ErrorMessage = "Не указано название средства измерения")]
        public string DeviceName { get; set; }
        [Required(ErrorMessage = "Не указан тип средства измерения")]
        public string DeviceType { get; set; }
        [Required(ErrorMessage = "Не указан заводской номер средства измерения")]
        public string SerialNumber { get; set; }

        public string RegistrationNumber { get; set; }
        public string VerificationMethodic { get; set; }

        public DateTime CalibrationDate { get; set; }
        public DateTime CalibrationExpireDate { get; set; }
        public DateTime DocumentDate { get; set; }

        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        [Required(ErrorMessage = "Не выбран файл документа")]
        public IFormFile DocumentFile { get; set; }
    }
}
