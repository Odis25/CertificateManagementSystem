using CertificateManagementSystem.Data.Models;
using CertificateManagementSystem.Extensions;
using CertificateManagementSystem.Models.Account;
using CertificateManagementSystem.Services.Components;
using CertificateManagementSystem.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CertificateManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(IUserService users, SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _userService = users;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // Страница входа в приложение
        public IActionResult Login()
        {
            return View(new LoginModel());
        }

        // Вход в приложение
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Получаем пользователя
                    var user = await _userService.Login(model.UserLogin, model.UserPassword);
                    if (user == null)
                    {
                        ModelState.AddModelError("", "Неправильный логин и(или) пароль.");
                        return View();
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

        // Выход из приложения
        public async Task<IActionResult> Logout()
        {
            var returnUrl = HttpContext.Request.Headers["Referer"];
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }

        // Управление правами пользователей
        [Authorize(Roles = "Admin, Metrologist, Specialist")]
        public IActionResult ManageAccounts()
        {
            var users = _userService.GetApplicationUsers();
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

        // Изменение прав пользователей
        [HttpPost]
        public async Task<IActionResult> ManageAccounts(ManageAccountsModel model)
        {
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                foreach (var user in model.Users)
                {
                    await _userService.ChangeUserRole(user.Id, user.Role.ToString());
                    if (user.Id == currentUser.Id)
                    {
                        await _signInManager.RefreshSignInAsync(currentUser);
                    }
                }
                this.AddAlertSuccess("Изменения сохранены");
            }
            catch(Exception e)
            {
                this.AddAlertDanger(e.Message);
            }
            return RedirectToAction("ManageAccounts");
        }

        // Доступ запрещен
        public IActionResult AccessDenied()
        {
            return View();
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
