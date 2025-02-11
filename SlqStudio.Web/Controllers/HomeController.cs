using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SlqStudio.Application.Services;
using SlqStudio.Models;

namespace SlqStudio.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IJwtTokenHandler _jwtTokenHandler;

    public HomeController(ILogger<HomeController> logger, IJwtTokenHandler jwtTokenHandler)
    {
        _logger = logger;
        _jwtTokenHandler = jwtTokenHandler;
    }

    // Действие Index
    public IActionResult Index()
    {
        // Например, предполагаем, что токен сохранен в cookies
        var token = Request.Cookies["jwt"];
        
        // Получаем данные из токена
        var (email, role) = _jwtTokenHandler.GetClaimsFromToken(token);
        
        // Сохраняем в сессию
        HttpContext.Session.SetString("UserEmail", email);
        HttpContext.Session.SetString("UserRole", role.ToString());

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}