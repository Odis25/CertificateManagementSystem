using CertificateManagementSystem.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Data
{
    public class DataSeeder
    {
        private readonly ApplicationDbContext _context;

        public DataSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public void SeedDataBase()
        {           
            SeedRoles().Wait();
            SeedSuperUser().Wait();
        }

        private async Task SeedRoles()
        {
            var roleStore = new RoleStore<IdentityRole>(_context);

            var hasAdminRole = _context.Roles.Any(r => r.Name == "Admin");
            var hasMetrologistRole = _context.Roles.Any(r => r.Name == "Metrologist");
            var hasSpecialistRole = _context.Roles.Any(r => r.Name == "Specialist");
            var hasUserRole = _context.Roles.Any(r => r.Name == "User");

            if (!hasAdminRole)
                await roleStore.CreateAsync(new IdentityRole { Name = "Admin", NormalizedName = "admin" });

            if (!hasMetrologistRole)
                await roleStore.CreateAsync(new IdentityRole { Name = "Metrologist", NormalizedName = "metrologist" });

            if (!hasSpecialistRole)
                await roleStore.CreateAsync(new IdentityRole { Name = "Specialist", NormalizedName = "specialist" });
            
            if (!hasUserRole)
                await roleStore.CreateAsync(new IdentityRole { Name = "User", NormalizedName = "user" });

            await _context.SaveChangesAsync();
        }

        private async Task SeedSuperUser()
        {
            var userStore = new UserStore<ApplicationUser>(_context);
            var roleStore = new RoleStore<IdentityRole>(_context);

            var hasSuperUser = _context.ApplicationUsers.Any(u => u.NormalizedUserName == "budanovav");

            var user = new ApplicationUser
            {
                UserName = "Budanovav",                
                NormalizedUserName = "budanovav",
                Email = "artem.budanov@incomsystem.ru",
                NormalizedEmail = "artem.budanov@incomsystem.ru",
                FullName = "Буданов Артем Владимирович",
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
           
            if (!hasSuperUser)
            {
                await userStore.CreateAsync(user);
                await userStore.AddToRoleAsync(user, "Admin");
            }
                
            await _context.SaveChangesAsync();
        }
    }
}
