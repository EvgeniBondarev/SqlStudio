namespace SlqStudio.Persistence.Models;

public class LabWork
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Number { get; set; }
    
    public int CourseId { get; set; }
    public Course Course { get; set; }
    
    public ICollection<LabTask> Tasks { get; set; } = new List<LabTask>();
}