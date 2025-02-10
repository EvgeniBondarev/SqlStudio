using Microsoft.AspNetCore.Mvc;
using SlqStudio.Application.Services;
using SlqStudio.ViewModels.Auth;

namespace SlqStudio.Controllers
{
    public class AuthController : Controller
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IJwtTokenHandler _jwtTokenHandler;

        public AuthController(IJwtTokenService jwtTokenService, IJwtTokenHandler jwtTokenHandler)
        {
            _jwtTokenService = jwtTokenService;
            _jwtTokenHandler = jwtTokenHandler;
        }

        public IActionResult Login()
        {
            var token = Request.Cookies["jwt"];
            if (token != null && _jwtTokenService.ValidateJwtToken(token))
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginRequest request)
        {
            if (request.Username != "admin" || request.Password != "admin")
                return View();

            var tokenString = _jwtTokenService.GenerateJwtToken(request.Username);
            Response.Cookies.Append("jwt", tokenString, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(30)
            });
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login");
        }
    }
}
