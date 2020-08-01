using Microsoft.AspNetCore.Identity;

namespace CertificateManagementSystem.Services.Models
{
    public class ApplicationUserDTO : IdentityUser
    {
        public string FullName { get; set; }
    }
}
