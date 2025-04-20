using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using SlqStudio.Application.Services.AppSettingsServices;

namespace SlqStudio.Controllers;

public class ConfigController : BaseMvcController
{
    private readonly IAppSettingsService _appSettingsService;
    private readonly AppSettingsBuilder _appSettingsBuilder;
    private readonly string _appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

    public ConfigController(
        IAppSettingsService appSettingsService,
        AppSettingsBuilder appSettingsBuilder,
        ILogger<ConfigController> logger
    ) : base(logger)
    {
        _appSettingsService = appSettingsService;
        _appSettingsBuilder = appSettingsBuilder;
    }

    public IActionResult Login()
    {
        LogInfo("Открыта страница входа.");
        return View();
    }

    [HttpPost]
    public IActionResult Login(string name, string password)
    {
        LogInfo("Попытка входа", new { name });

        try
        {
            var config = _appSettingsService.ReadConfig(_appSettingsPath);
            var userSection = config["ConfigUser"];

            if (userSection != null &&
                name == userSection["Name"]?.ToString() &&
                password == userSection["Password"]?.ToString())
            {
                HttpContext.Session.SetString("IsConfigAuthorized", "true");
                LogInfo("Успешный вход", new { name });
                return RedirectToAction("EditConfig");
            }

            LogWarning("Неуспешный вход: неверные данные", new { name });
            ViewBag.Error = "Неверное имя пользователя или пароль.";
            return View();
        }
        catch (Exception ex)
        {
            LogError("Ошибка при попытке входа", ex);
            throw;
        }
    }

    public IActionResult EditConfig()
    {
        if (HttpContext.Session.GetString("IsConfigAuthorized") != "true")
        {
            LogWarning("Попытка доступа к редактированию без авторизации");
            return RedirectToAction("Login");
        }

        try
        {
            var jsonObj = _appSettingsService.ReadConfig(_appSettingsPath);
            return View(jsonObj);
        }
        catch (Exception ex)
        {
            LogError("Ошибка при загрузке конфигурации", ex);
            throw;
        }
    }

    [HttpPost]
    public IActionResult SaveConfig(IFormCollection form)
    {
        try
        {
            var updatedConfig = _appSettingsBuilder.BuildAppSettings(form);
            _appSettingsService.WriteConfig(updatedConfig, _appSettingsPath);
            LogInfo("Конфигурация успешно сохранена", new { Keys = form.Keys });
            return RedirectToAction("EditConfig");
        }
        catch (Exception ex)
        {
            LogError("Ошибка при сохранении конфигурации", ex);
            throw;
        }
    }
}
