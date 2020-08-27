using System.ComponentModel.DataAnnotations;

namespace CertificateManagementSystem.Services.ViewModels.Account
{
    public class LoginModel
    {
        [Display(Name = "Имя пользователя")]
        [Required(ErrorMessage = "Не указано имя пользователя.")]
        public string UserLogin { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Не указан пароль.")]
        [DataType(DataType.Password)]
        public string UserPassword { get; set; }

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}
