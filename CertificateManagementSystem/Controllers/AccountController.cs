using CertificateManagementSystem.Data;
using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Models.Account;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
        //private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(ILDAPService users, SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)//, RoleManager<IdentityRole> roleManager)
        {
            _users = users;
            _signInManager = signInManager;
            _userManager = userManager;
            //_roleManager = roleManager;
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
                    var user = _users.Login(model.UserLogin, model.UserPassword);
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
            });
            var model = new ManageAccountsModel
            {
                Users = result
            };

            return View(model);
        }

        private async Task<string> GetRole(ApplicationUser user)
        {

            return (await _userManager.GetRolesAsync(user)).FirstOrDefault();
        }
    }
}
