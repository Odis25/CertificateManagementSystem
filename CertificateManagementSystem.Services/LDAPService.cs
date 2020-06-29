using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;

namespace CertificateManagementSystem.Services
{
    public class LDAPService : ILDAPService
    {
        private const string DisplayNameAttribute = "DisplayName";
        private const string GivenNameAttribute = "GivenName";
        private const string SnAttribute = "Sn";
        private const string MailAttribute = "Mail";
        private const string SAMAccountNameAttribute = "SAMAccountName";

        private readonly ApplicationDbContext _context;

        public LDAPService(ApplicationDbContext context)
        {
            _context = context;
        }

        public ApplicationUser Login(string userName, string userPassword)
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
                            //searcher.PropertiesToLoad.Add(MailAttribute);

                            var result = searcher.FindOne();

                            if (result != null)
                            {
                                var accountName = result.Properties[SAMAccountNameAttribute][0].ToString();
                                var displayName = result.Properties[DisplayNameAttribute][0].ToString();
                                //var email = result.Properties[MailAttribute][0].ToString();

                                var user = _context.ApplicationUsers.FirstOrDefault(u => u.UserName == accountName) ??
                                    new ApplicationUser
                                    {
                                        UserName = accountName,
                                        NormalizedUserName = accountName.ToLower(),
                                        //Email = email,
                                        //NormalizedEmail = email.ToLower(),
                                        ConcurrencyStamp = Guid.NewGuid().ToString(),
                                        FullName = displayName
                                    };

                                return user;
                            }
                        }
                    }
                }
            }

            return null;
        }

        public IEnumerable<ApplicationUser> GetApplicationUsers()
        {
            using (var entry = new DirectoryEntry("LDAP://incomsystem.ru", "INCOMSYSTEM\\budanovav", "Narutaru25@"))
            {
                using (var searcher = new DirectorySearcher(entry))
                {
                    searcher.Filter = $"(&(objectClass=user)(sn=*))";
                    // Получаемые свойства
                    searcher.PropertiesToLoad.Add(SAMAccountNameAttribute);
                    searcher.PropertiesToLoad.Add(DisplayNameAttribute);
                    searcher.PropertiesToLoad.Add(GivenNameAttribute);
                    searcher.PropertiesToLoad.Add(SnAttribute);
                    //searcher.PropertiesToLoad.Add(MailAttribute);

                    var users = searcher.FindAll();
                    var appUsers = new List<ApplicationUser>();

                    foreach (SearchResult user in users)
                    {
                        var accountName = user.Properties[SAMAccountNameAttribute][0].ToString();
                        var displayName = user.Properties[DisplayNameAttribute][0].ToString();
                        //var email = user.Properties[MailAttribute][0].ToString();

                        var appUser = new ApplicationUser
                        {
                            UserName = accountName,
                            NormalizedUserName = accountName.ToLower(),
                            //Email = email,
                            //NormalizedEmail = email.ToLower(),
                            ConcurrencyStamp = Guid.NewGuid().ToString(),
                            FullName = displayName
                        };
                        appUsers.Add(appUser);
                    }
                    return appUsers;
                }
            }
        }
    }
}

