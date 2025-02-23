namespace SlqStudio.ViewModel.Models;

public class LabTaskViewModel
{
    public int Id { get; set; }
    public int Number { get; set; }
    public string Title { get; set; }
    public string Condition { get; set; }
    public List<Dictionary<string, object>>? QueryData { get; set; }
}