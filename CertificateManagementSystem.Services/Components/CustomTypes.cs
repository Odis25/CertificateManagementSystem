using System.ComponentModel.DataAnnotations;

namespace CertificateManagementSystem.Services.Components
{
    public enum DocumentType
    {
        [Display(Name = "Свидетельство о поверке")]
        Certificate,
        [Display(Name = "Извещение о непригодности")]
        FailureNotification
    }

    public enum UserRole
    {
        [Display(Name ="Пользователь")]
        User,
        [Display(Name = "Сотрудник ТОиС")]
        Specialist,
        [Display(Name = "Метролог")]
        Metrologist,
        [Display(Name = "Администратор")]
        Admin
    }
}
