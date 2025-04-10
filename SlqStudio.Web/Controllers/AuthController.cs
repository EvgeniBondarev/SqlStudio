using Microsoft.AspNetCore.Mvc;
using SlqStudio.Application.ApiClients.Moodle;
using SlqStudio.Application.Services;
using SlqStudio.ViewModels.Auth;

namespace SlqStudio.Controllers
{
    public class AuthController : Controller
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IJwtTokenHandler _jwtTokenHandler;
        private readonly IMoodleService _moodleService;

        public AuthController(IJwtTokenService jwtTokenService,
                            IJwtTokenHandler jwtTokenHandler, 
                            IMoodleService moodleService)
        {
            _jwtTokenService = jwtTokenService;
            _jwtTokenHandler = jwtTokenHandler;
            _moodleService = moodleService;
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
        public async Task<IActionResult> Login(LoginRequest request)
        {
            /*var userData = await _moodleService.GetUserByEmailAsync(request.Email);
            if (userData == null)
            {
                return View();
            }
            var course = await _moodleService.GetAllCourseByName("Базы данных");
            var userRole = await _moodleService.GetUserProfileAsync(userData!.Id, course.Id);
            var tokenString = _jwtTokenService.GenerateJwtToken(userData.Email, 
                                                            userRole.Roles.FirstOrDefault().ShortName,
                                                                userData.FullName);*/
            var tokenString = _jwtTokenService.GenerateJwtToken("test", 
                "editingteacher",
                "test");
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
