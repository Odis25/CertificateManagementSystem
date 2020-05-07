using CertificateManagementSystem.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CertificateManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            //Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Contract> Contracts { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<FailureNotification> FailureNotifications { get; set; }
        public DbSet<VerificationMethodic> VerificationMethodics { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    }
}
