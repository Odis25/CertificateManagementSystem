using Microsoft.AspNetCore.Identity;

namespace CertificateManagementSystem.Data.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string FullName { get; set; }
    }
}
