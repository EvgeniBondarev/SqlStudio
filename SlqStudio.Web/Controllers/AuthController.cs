using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SlqStudio.Application.ApiClients.Moodle;
using SlqStudio.Application.CQRS.Course.Queries;
using SlqStudio.Application.Services;
using SlqStudio.Persistence.Models;
using SlqStudio.ViewModels.Auth;

namespace SlqStudio.Controllers
{
    public class AuthController : Controller
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IMediator _mediator;
        private readonly IMoodleService _moodleService;

        public AuthController(
            IJwtTokenService jwtTokenService,
            IMediator mediator,
            IMoodleService moodleService)
        {
            _jwtTokenService = jwtTokenService;
            _mediator = mediator;
            _moodleService = moodleService;
        }

        [AllowAnonymous]
        public async Task<ActionResult> Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToLocal(returnUrl);
            }

            try
            {
                await LoadCoursesToViewBagAsync();
                ViewBag.ReturnUrl = returnUrl;
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Произошла ошибка при загрузке данных: " + ex.Message;
                return View();
            }
        }


        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequest request, string? returnUrl = null)
        {
            await LoadCoursesToViewBagAsync();

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Пожалуйста, заполните все обязательные поля корректно.";
                return View(request);
            }

            try
            {
                var userData = await _moodleService.GetUserByEmailAsync(request.Email);
                if (userData == null)
                {
                    TempData["ErrorMessage"] = "Пользователь с таким email не найден.";
                    return View(request);
                }

                if (string.IsNullOrEmpty(request.Course))
                {
                    TempData["ErrorMessage"] = "Курс не выбран.";
                    return View(request);
                }

                var course = await _moodleService.GetAllCourseByName(request.Course);
                if (course == null)
                {
                    TempData["ErrorMessage"] = "Выбранный курс не найден.";
                    return View(request);
                }

                var userRole = await _moodleService.GetUserProfileAsync(userData.Id, course.Id);
                if (userRole == null || userRole.Roles == null || !userRole.Roles.Any())
                {
                    TempData["ErrorMessage"] = "Не удалось определить вашу роль в системе.";
                    return View(request);
                }

                var tokenString = _jwtTokenService.GenerateJwtToken(
                    userData.Email,
                    userRole.Roles.FirstOrDefault()?.ShortName ?? string.Empty,
                    userData.FullName);

                Response.Cookies.Append("jwt", tokenString, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(30)
                });

                return RedirectToLocal(returnUrl);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Произошла ошибка при авторизации: " + ex.Message;
                return View(request);
            }
        }


        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login", "Auth");
        }

        private ActionResult RedirectToLocal(string? returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
        private async Task LoadCoursesToViewBagAsync()
        {
            var courses = await _mediator.Send(new GetAllCoursesQuery());
            if (courses == null || !courses.Any())
            {
                TempData["ErrorMessage"] = "Не удалось загрузить список курсов. Пожалуйста, попробуйте позже.";
            }

            ViewBag.Courses = new SelectList(courses, "Name", "Name");
        }

    }
}