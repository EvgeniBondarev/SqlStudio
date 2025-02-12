namespace SlqStudio.Persistence.Models;

public class LabTask
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string Title { get; set; }
    public string Condition { get; set; }
    public string SolutionExample { get; set; }
    
    public int LabWorkId { get; set; }
    public LabWork LabWork { get; set; }
}
