using CertificateManagementSystem.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CertificateManagementSystem.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<FailureNotification> FailureNotifications { get; set; }
        public DbSet<VerificationMethodic> VerificationMethodics { get; set; }
    }
}
