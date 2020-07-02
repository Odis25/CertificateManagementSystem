using System.ComponentModel.DataAnnotations;

namespace CertificateManagementSystem.Services.Components
{
    public enum DocumentType
    {
        Certificate,
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
