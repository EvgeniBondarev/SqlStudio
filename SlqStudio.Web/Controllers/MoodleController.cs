using Microsoft.AspNetCore.Mvc;
using SlqStudio.Application.ApiClients.Moodle;

namespace SlqStudio.Controllers;

[ApiController]
[Route("api/moodle")]
public class MoodleController : BaseMvcController
{
    private readonly IMoodleService _moodleService;

    public MoodleController(
        ILogger<MoodleController> logger,
        IMoodleService moodleService)
        : base(logger)
    {
        _moodleService = moodleService;
    }

    [HttpGet("user-profile")]
    public async Task<IActionResult> GetUserProfile([FromQuery] int userId, [FromQuery] int courseId)
    {
        try
        {
            LogInfo($"Запрос на получение профиля пользователя. UserId: {userId}, CourseId: {courseId}");
            var response = await _moodleService.GetUserProfileAsync(userId, courseId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            LogError($"Ошибка при получении профиля пользователя. UserId: {userId}, CourseId: {courseId}", ex);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
    
    [HttpGet("all-courses")]
    public async Task<IActionResult> GetAllCourses()
    {
        try
        {
            LogInfo("Запрос на получение всех курсов");
            var response = await _moodleService.GetAllCoursesAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            LogError("Ошибка при получении списка курсов", ex);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("user-by-email")]
    public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
    {
        try
        {
            LogInfo($"Запрос на получение пользователя по email: {email}");
            var response = await _moodleService.GetUserByEmailAsync(email);
            return Ok(response);
        }
        catch (Exception ex)
        {
            LogError($"Ошибка при получении пользователя по email: {email}", ex);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}
