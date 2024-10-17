using AppsettingsEdition.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace AppsettingsEdition.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _currentDirectory = Environment.CurrentDirectory;
        private readonly string _appsettingsPath;
        private readonly string _appsettingsBackupsPath;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
            _appsettingsPath = Path.Combine(_currentDirectory, "appsettings.json");
            _appsettingsBackupsPath = Path.Combine(_currentDirectory, "AppsettingsBackups");
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditAppsettings()
        {
            var model = new EditAppsettingsViewModel()
            {
                OldValue = System.IO.File.ReadAllText(_appsettingsPath)
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditAppsettings(EditAppsettingsViewModel model)
        {
            if (!await IsValidJsonAsync(model.NewValue))
            {
                ModelState.AddModelError("", "Неверный формат. Данные должны быть в формате json");
                model.OldValue = model.NewValue;
                return View(model);
            }
            CreateAppsettingsBackup();
            System.IO.File.WriteAllText(_appsettingsPath, model.NewValue);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> IsValidJsonAsync(string json)
        {
            try
            {
                var obj = JsonConvert.DeserializeObject<object>(json);
                return true; 
            }
            catch (Newtonsoft.Json.JsonException ex)
            {
                return false;
            }
        }

        private void CreateAppsettingsBackup()
        {
            if (!Directory.Exists(_appsettingsBackupsPath))
            {
                Directory.CreateDirectory(_appsettingsBackupsPath);
            }
            var currentDate = DateTime.Now;
            var backupFileName = $"appsettings_{currentDate.ToString("s").Replace(':', '_')}.json";
            System.IO.File.Copy(_appsettingsPath, Path.Combine(_appsettingsBackupsPath, backupFileName), overwrite: true);
        }
    }
}
