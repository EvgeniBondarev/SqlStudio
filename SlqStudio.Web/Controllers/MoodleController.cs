using Microsoft.AspNetCore.Mvc;
using SlqStudio.Application.ApiClients.Moodle;

namespace SlqStudio.Controllers;

[ApiController]
[Route("api/moodle")]
public class MoodleController : ControllerBase
{
    private readonly IMoodleService _moodleService;
    private readonly ILogger<MoodleController> _logger;

    public MoodleController(
        IMoodleService moodleService,
        ILogger<MoodleController> logger)
    {
        _moodleService = moodleService;
        _logger = logger;
    }

    [HttpGet("user-profile")]
    public async Task<IActionResult> GetUserProfile([FromQuery] int userId, [FromQuery] int courseId)
    {
        try
        {
            var response = await _moodleService.GetUserProfileAsync(userId, courseId);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user profile");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
    
    [HttpGet("all-courses")]
    public async Task<IActionResult> GetAllCourses()
    {
        try
        {
            var response = await _moodleService.GetAllCoursesAsync();
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching courses");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    [HttpGet("user-by-email")]
    public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
    {
        try
        {
            var response = await _moodleService.GetUserByEmailAsync(email);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching user by email");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }
}