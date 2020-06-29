using CertificateManagementSystem.Data.Models;
using System.Collections.Generic;

namespace CertificateManagementSystem.Data
{
    public interface ILDAPService
    {
        ApplicationUser Login(string userName, string userPassword);
        IEnumerable<ApplicationUser> GetApplicationUsers();
    }
}
