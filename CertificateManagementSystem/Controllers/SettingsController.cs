using CertificateManagementSystem.Extensions;
using CertificateManagementSystem.Services.ViewModels.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace CertificateManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SettingsController : Controller
    {
        private IOptions<SettingsModel> _options;

        public SettingsController(IOptions<SettingsModel> options)
        {
            _options = options;
        }

        // Открываем окно настроек
        public IActionResult Index()
        {
            var model = new SettingsModel
            {
                DocumentsFolder = _options.Value.DocumentsFolder,
                ReserveFolder = _options.Value.ReserveFolder,
                MethodicsFolder = _options.Value.MethodicsFolder
            };
            return View(model);
        }

        // Изменяем настройки
        [HttpPost]
        public IActionResult SaveSettings(SettingsModel model)
        {
            if (!Directory.Exists(model.DocumentsFolder))
            {
                ModelState.AddModelError("", "Указанный каталог хранения документов не существует");
            }
            if (!Directory.Exists(model.ReserveFolder))
            {
                ModelState.AddModelError("", "Указанный резервный каталог хранения документов не существует");
            }
            if (!Directory.Exists(model.MethodicsFolder))
            {
                ModelState.AddModelError("", "Указанный каталог хранения методик поверки не существует");
            }

            if (ModelState.IsValid)
            {
                // Читаем файл настроек
                var jsonString = System.IO.File.ReadAllText("appsettings.json");

                // Преобразовываем его в JSON-объект
                JObject jsonObject = JObject.Parse(jsonString);
                JObject paths = (JObject)jsonObject["Paths"];

                // Меняем значения 
                _options.Value.DocumentsFolder = model.DocumentsFolder;
                _options.Value.ReserveFolder = model.ReserveFolder;
                _options.Value.MethodicsFolder = model.MethodicsFolder;

                paths["DocumentsFolder"] = _options.Value.DocumentsFolder;
                paths["ReserveFolder"] = _options.Value.ReserveFolder;
                paths["MethodicsFolder"] = _options.Value.MethodicsFolder;

                // Преобразовываем JSON-объект в строку
                jsonString = JsonConvert.SerializeObject(jsonObject);

                // Сохраняем файл с новыми настройками
                System.IO.File.WriteAllText("appsettings.json", jsonString);

                this.AddAlertSuccess("Новые настройки успешно сохранены");

                return RedirectToAction("Index", "Settings");
            }
            else
            {
                foreach (var error in ModelState[""].Errors)
                {
                    this.AddAlertDanger(error.ErrorMessage);
                }

                return View("Index");
            }
        }
    }
}
