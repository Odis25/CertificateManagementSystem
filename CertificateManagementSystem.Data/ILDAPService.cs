using CertificateManagementSystem.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Data
{
    public interface ILDAPService
    {
        Task<ApplicationUser> Login(string userName, string userPassword);
        IEnumerable<ApplicationUser> GetApplicationUsers();
    }
}
