using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Services
{
    public class UserService : IUserService
    {
        private const string DisplayNameAttribute = "DisplayName";
        private const string GivenNameAttribute = "GivenName";
        private const string SnAttribute = "Sn";
        private const string MailAttribute = "Mail";
        private const string SAMAccountNameAttribute = "SAMAccountName";

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser> Login(string userName, string userPassword)
        {
            using (var context = new PrincipalContext(ContextType.Domain, "INCOMSYSTEM.ru"))
            {
                // Проверка логина и пароля
                if (context.ValidateCredentials(userName, userPassword))
                {
                    // Поиск пользователя в ActiveDirectory
                    using (var entry = new DirectoryEntry("LDAP://incomsystem.ru", "INCOMSYSTEM\\" + userName, userPassword))
                    {
                        using (var searcher = new DirectorySearcher(entry))
                        {
                            // Критерий поиска
                            searcher.Filter = $"SAMAccountName={userName}";

                            // Получаемые свойства
                            searcher.PropertiesToLoad.Add(SAMAccountNameAttribute);
                            searcher.PropertiesToLoad.Add(DisplayNameAttribute);
                            searcher.PropertiesToLoad.Add(GivenNameAttribute);
                            searcher.PropertiesToLoad.Add(SnAttribute);

                            var result = searcher.FindOne();

                            if (result != null)
                            {
                                var accountName = result.Properties[SAMAccountNameAttribute][0].ToString();
                                var displayName = result.Properties[DisplayNameAttribute][0].ToString();

                                var user = _context.ApplicationUsers.FirstOrDefault(u => u.UserName == accountName);
                                if (user == null)
                                {
                                    user = new ApplicationUser
                                    {
                                        UserName = accountName,
                                        NormalizedUserName = accountName.ToLower(),
                                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                                        FullName = displayName
                                    };
                                    var userStore = new UserStore<ApplicationUser>(_context);
                                    await userStore.CreateAsync(user);
                                    await _userManager.AddToRoleAsync(user, "User");
                                }
                                return user;
                            }
                        }
                    }
                }
            }

            return null;
        }

        // Изменить роль пользователя
        public async Task ChangeUserRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var role = (await _userManager.GetRolesAsync(user))[0];

            if (role == newRole)
                return;

            await _userManager.RemoveFromRoleAsync(user, role);
            await _userManager.AddToRoleAsync(user, newRole);
        }

        public IEnumerable<ApplicationUser> GetApplicationUsers()
        {
            return _context.ApplicationUsers;
        }
    }
}

