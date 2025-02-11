namespace SlqStudio.Application.ApiClients.Moodle.Models;

public class MoodleUserProfileResponse
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string ProfileImageUrl { get; set; }
    public List<MoodleUserRole> Roles { get; set; } = new List<MoodleUserRole>();
    public List<MoodleUserCourse> EnrolledCourses { get; set; } = new List<MoodleUserCourse>();
}