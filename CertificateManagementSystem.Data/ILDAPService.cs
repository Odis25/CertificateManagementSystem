using CertificateManagementSystem.Data.Models;

namespace CertificateManagementSystem.Data
{
    public interface ILDAPService
    {
        ApplicationUser Login(string userName, string userPassword);
    }
}
