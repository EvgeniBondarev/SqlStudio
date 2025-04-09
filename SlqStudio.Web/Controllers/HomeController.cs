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

    public IActionResult Index()
    {
        var token = Request.Cookies["jwt"];
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login", "Auth");
        }
        try
        {
            var (email, role, name) = _jwtTokenHandler.GetClaimsFromToken(token);
            HttpContext.Session.SetString("UserEmail", email);
            HttpContext.Session.SetString("UserRole", role.ToString());
            HttpContext.Session.SetString("UserName", name);

            return View();
        }
        catch (Exception ex)
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login", "Auth");
        }
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