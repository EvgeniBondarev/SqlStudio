using Microsoft.AspNetCore.Mvc;
using SlqStudio.Application.ApiClients.Moodle;

namespace SlqStudio.Controllers;

[ApiController]
[Route("api/moodle")]
public class MoodleController : ControllerBase
{
    private readonly MoodleApiClient _moodleApiClient;
    private readonly ILogger<MoodleController> _logger;

    public MoodleController(MoodleApiClient moodleApiClient, ILogger<MoodleController> logger)
    {
        _moodleApiClient = moodleApiClient;
        _logger = logger;
    }

    [HttpGet("user-profile")]
    public async Task<IActionResult> GetUserProfile([FromQuery] int userId, [FromQuery] int courseId)
    {
        try
        {
            var parameters = new Dictionary<string, string>
            {
                { "userlist[0][userid]", userId.ToString() },
                { "userlist[0][courseid]", courseId.ToString() }
            };

            var response = await _moodleApiClient.SendRequestAsync<MoodleUserProfileResponse>(
                "core_user_get_course_user_profiles", parameters);

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
            var parameters = new Dictionary<string, string>();
            var response = await _moodleApiClient.SendRequestAsync<MoodleCourseResponse>(
                "core_course_get_courses", parameters);
            return Ok(response);
        }
        catch (Exception ex)
        {
            
            _logger.LogError(ex, "Error fetching courses");
            return StatusCode(500, new { message = "Internal server error" });
        }
    }


}