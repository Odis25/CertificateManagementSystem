using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Extensions;
using CertificateManagementSystem.Models.Account;
using CertificateManagementSystem.Services.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ILDAPService _users;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(ILDAPService users, SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _users = users;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Получаем пользователя
                    var user = await _users.Login(model.UserLogin, model.UserPassword);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "Неправильный логин и(или) пароль.");
                        return View();
                    }

                    // Проверяем роль пользователя
                    var roles = await _userManager.GetRolesAsync(user);
                    if (!roles.Any())
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }

                    // Авторизуем пользователя в программе
                    await _signInManager.SignInAsync(user, model.RememberMe);

                    return RedirectToAction("Index", "Document");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            var returnUrl = HttpContext.Request.Headers["Referer"];
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }

        // Управление правами пользователей
        public IActionResult ManageAccounts()
        {
            var users = _users.GetApplicationUsers();
            var result = users.Select(u => new UserModel
            {
                Id = u.Id,
                AccountName = u.UserName,
                FullName = u.FullName,
                Role = GetRole(u).Result
            }).ToList();

            var model = new ManageAccountsModel
            {
                Users = result
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ManageAccounts(ManageAccountsModel model)
        {
            try
            {
                foreach (var user in model.Users)
                {
                    await ChangeUserRole(user.Id, user.Role.ToString());
                }
                this.AddAlertSuccess("Изменения сохранены");
            }
            catch(Exception e)
            {
                this.AddAlertDanger(e.Message);
            }
            return RedirectToAction("ManageAccounts");
        }

        // Изменить роль пользователя
        private async Task ChangeUserRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var role = (await _userManager.GetRolesAsync(user))[0];

            if (role == newRole)
                return;

            await _userManager.RemoveFromRoleAsync(user, role);
            await _userManager.AddToRoleAsync(user, newRole);

        }
        // Получить роль пользователя
        private async Task<UserRole> GetRole(ApplicationUser user)
        {
            var role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            return role switch
            {
                "Admin" => UserRole.Admin,
                "Metrologist" => UserRole.Metrologist,
                "Specialist" => UserRole.Specialist,
                "User" => UserRole.User,
                _ => UserRole.User,
            };
        }
    }
}
