using CertificateManagementSystem.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Data
{
    public interface IUserService
    {
        Task ChangeUserRole(string userId, string newRole);
        Task<ApplicationUser> Login(string userName, string userPassword);
        IEnumerable<ApplicationUser> GetApplicationUsers();
    }
}
