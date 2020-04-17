using CertificateManagementSystem.Models.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CertificateManagementSystem.Controllers
{
    public class SettingsController: Controller
    {
        private IOptions<ApplicationOptions> _options;

        public SettingsController(IOptions<ApplicationOptions> options )
        {
            _options = options;
        }

        // Открываем окно настроек
        public IActionResult Index()
        {
            var documentFolder = _options.Value.DocumentsFolder;
            var reserveFolder = _options.Value.ReserveFolder;
            var methodicsFolder = _options.Value.MethodicsFolder;

            var model = new SettingsModel
            {
                DocumentsFolder = documentFolder,
                ReserveFolder = reserveFolder,
                MethodicsFolder = methodicsFolder
            };
            return View(model);
        }

        // Изменяем настройки
        [HttpPost]
        public IActionResult SaveSettings(SettingsModel model)
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

            return RedirectToAction("Index", "Home");
        }

        public string GetFolderPath()
        {
             
            return "hello";
        }
    }
}
