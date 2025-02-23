using SlqStudio.Persistence.Models;
using SlqStudio.ViewModel.Models;

namespace SlqStudio.ViewModel.Mappers;

public static class LabTaskMapper
{
    public static LabTaskViewModel ToViewModel(this LabTask labTask)
    {
        return new LabTaskViewModel
        {
            Id = labTask.Id,
            Number = labTask.Number,
            Title = labTask.Title,
            Condition = labTask.Condition,
            QueryData = null
        };
    }
}