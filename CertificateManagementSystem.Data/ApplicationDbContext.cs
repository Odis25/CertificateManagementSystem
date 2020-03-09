using Microsoft.EntityFrameworkCore;

namespace CertificateManagementSystem.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

    }
}
