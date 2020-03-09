using Microsoft.EntityFrameworkCore;

namespace CertificateManagementSystem.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext()
        {
            Database.EnsureCreated();
        }

    }
}
