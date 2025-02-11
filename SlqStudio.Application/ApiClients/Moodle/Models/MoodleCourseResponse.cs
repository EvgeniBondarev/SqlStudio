namespace SlqStudio.Application.ApiClients.Moodle.Models;

public class MoodleCourseResponse
{
    public int Id { get; set; }
    public string Shortname { get; set; }
    public int CategoryId { get; set; }
    public string Fullname { get; set; }
    public string Displayname { get; set; }
    public string Summary { get; set; }
    public int StartDate { get; set; }
    public int EndDate { get; set; }
    public int Visible { get; set; }
}
