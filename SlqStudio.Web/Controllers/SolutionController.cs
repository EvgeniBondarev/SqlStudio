using Application.Common.SQL;
using Application.Common.SQL.ResponseModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SlqStudio.Application.CQRS.LabTask.Queries;
using SlqStudio.Session;
using SlqStudio.ViewModel.Mappers;
using SlqStudio.ViewModel.Models;
using SlqStudio.ViewModels;

namespace SlqStudio.Controllers;

public class SolutionController : Controller
{
    private readonly IMediator _mediator;
    private readonly SqlManager _sqlManager;

    public SolutionController(IMediator mediator, SqlManager sqlManager)
    {
        _mediator = mediator;
        _sqlManager = sqlManager;
    }
    
    public async Task<IActionResult> Index(int taskId)
    {
        var task = await _mediator.Send(new GetTaskByIdQuery(taskId));
        LabTaskViewModel  labTaskViewModel = task?.ToViewModel();
        labTaskViewModel.QueryData = await _sqlManager.ExecuteQueryAsync(task.SolutionExample);
        return View(labTaskViewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> ValidateSqlSyntax([FromBody] SqlRequestModel request)
    {
        var response = await _sqlManager.ValidateSqlSyntaxAsync(request.SqlQuery);

        if (response == null)
            return BadRequest("Не удалось обработать запрос.");

        return Ok(response);
    }


    [HttpPost]
    public async Task<IActionResult> CompareSql([FromBody] CompareSqlRequestModel request)
    {
        var task = await _mediator.Send(new GetTaskByIdQuery(request.TaskId));

        if (task == null)
            return NotFound($"Задача с ID {request.TaskId} не найдена.");

        var response = await _sqlManager.CompareSqlResultsAsync(task.SolutionExample, request.SqlQuery);

        if (response == null)
            return BadRequest("Ошибка сравнения данных SQL.");
        
        UpdateComparisonResultsInSession(request.TaskId, response.DataIsEqual, request.SqlQuery);

        return Ok(response);
    }

    private void UpdateComparisonResultsInSession(int taskId, bool result, string userSolution)
    {
        var comparisonResults = HttpContext.Session.GetObjectFromJson<List<ComparisonResult>>("ComparisonResults") ?? new List<ComparisonResult>();
        var existingResult = comparisonResults.FirstOrDefault(r => r.TaskId == taskId);

        if (existingResult != null)
        {
            existingResult.Result = result;
        }
        else
        {
            comparisonResults.Add(new ComparisonResult
            {
                TaskId = taskId,
                Result = result,
                UserSolutionText = userSolution
            });
        }
        HttpContext.Session.SetObjectAsJson("ComparisonResults", comparisonResults);
    }
}