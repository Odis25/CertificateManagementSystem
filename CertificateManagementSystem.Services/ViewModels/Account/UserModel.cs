using CertificateManagementSystem.Services.Components;

namespace CertificateManagementSystem.Services.ViewModels.Account
{
    public class UserModel
    {
        public string Id { get; set; }
        public string AccountName { get; set; }
        public string FullName { get; set; }
        public UserRole Role { get; set; }
    }
}
