using Application.Common.SQL;
using Application.Common.SQL.ResponseModels;
using Application.Common.SQL.Utils;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SlqStudio.Application.CQRS.LabTask.Queries;
using SlqStudio.Application.Services.VariantServices;
using SlqStudio.Session;
using SlqStudio.ViewModel.Mappers;
using SlqStudio.ViewModel.Models;
using SlqStudio.ViewModels;

namespace SlqStudio.Controllers;

public class SolutionController : BaseMvcController
{
    private readonly IMediator _mediator;
    private readonly SqlManager _sqlManager;
    private readonly VariantServices _variantServices;

    public SolutionController(IMediator mediator, 
                              SqlManager sqlManager,
                              VariantServices variantServices,
                              ILogger<SolutionController> logger)
        : base(logger)
    {
        _mediator = mediator;
        _sqlManager = sqlManager;
        _variantServices = variantServices;
    }

    public async Task<IActionResult> Index(int taskId)
    {
        try
        {
            LogInfo($"Запрос на отображение задания с ID {taskId}.");
            var variant = _variantServices.GetVariantFromCache(HttpContext.Session.GetString("UserEmail"));
            if (!variant.Any(v => v.Id == taskId))
            {
                LogWarning($"Доступ к задаче с ID {taskId} запрещен. Пользователь не имеет доступа.");
                return Forbid();
            }

            var task = await _mediator.Send(new GetTaskByIdQuery(taskId));
            if (task == null)
            {
                LogWarning($"Задача с ID {taskId} не найдена.");
                return NotFound();
            }

            LabTaskViewModel labTaskViewModel = task.ToViewModel();
            labTaskViewModel.QueryData = await _sqlManager.ExecuteQueryAsync(task.SolutionExample);
            LogInfo($"Задание с ID {taskId} успешно отображено.");
            return View(labTaskViewModel);
        }
        catch (Exception ex)
        {
            LogError($"Ошибка при отображении задания с ID {taskId}.", ex);
            return StatusCode(500, new { message = "Ошибка сервера." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> ValidateSqlSyntax([FromBody] SqlRequestModel request)
    {
        try
        {
            LogInfo("Запрос на проверку синтаксиса SQL.");
            var response = await _sqlManager.ValidateSqlSyntaxAsync(request.SqlQuery);

            if (response == null)
            {
                LogError("Ошибка при обработке SQL-запроса.");
                return BadRequest("Не удалось обработать запрос.");
            }

            LogInfo("Синтаксис SQL-запроса проверен успешно.");
            return Ok(response);
        }
        catch (Exception ex)
        {
            LogError("Ошибка при проверке синтаксиса SQL-запроса.", ex);
            return StatusCode(500, new { message = "Ошибка сервера." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> CompareSql([FromBody] CompareSqlRequestModel request)
    {
        try
        {
            LogInfo($"Запрос на сравнение SQL для задачи с ID {request.TaskId}.");
            var task = await _mediator.Send(new GetTaskByIdQuery(request.TaskId));

            if (task == null)
            {
                LogWarning($"Задача с ID {request.TaskId} не найдена.");
                return NotFound($"Задача с ID {request.TaskId} не найдена.");
            }

            var expectedType = SqlCommandTypeDetector.Detect(task.SolutionExample);
            var actualType = SqlCommandTypeDetector.Detect(request.SqlQuery);

            if (expectedType != actualType)
            {
                LogWarning($"Тип SQL-запроса не соответствует ожидаемому. Ожидался: {expectedType}, получен: {actualType}.");
                return BadRequest($"Тип запроса не соответствует ожидаемому. Ожидался: {expectedType}, получен: {actualType}.");
            }

            var response = await _sqlManager.CompareSqlResultsAsync(task.SolutionExample, request.SqlQuery);

            if (response == null)
            {
                LogError("Ошибка при сравнении SQL-запросов.");
                return BadRequest("Ошибка сравнения данных SQL.");
            }

            UpdateComparisonResultsInSession(request.TaskId, response.DataIsEqual, request.SqlQuery);
            LogInfo($"Сравнение SQL-запросов для задачи с ID {request.TaskId} завершено успешно.");
            return Ok(response);
        }
        catch (Exception ex)
        {
            LogError("Ошибка при сравнении SQL-запросов.", ex);
            return StatusCode(500, new { message = "Ошибка сервера." });
        }
    }

    private void UpdateComparisonResultsInSession(int taskId, bool result, string userSolution)
    {
        try
        {
            LogInfo($"Обновление результатов сравнения для задачи с ID {taskId}.");
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
            LogInfo($"Результаты сравнения для задачи с ID {taskId} успешно обновлены.");
        }
        catch (Exception ex)
        {
            LogError("Ошибка при обновлении результатов сравнения в сессии.", ex);
        }
    }
}
