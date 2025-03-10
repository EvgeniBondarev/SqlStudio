using SlqStudio.Persistence.Models;

namespace SlqStudio.DTO;

public class ReportDto
{
    public UserDto User { get; set; }
    public List<SolutionResultDto> Solutions { get; set; }
    public List<LabWork> LabWorks { get; set; }
}