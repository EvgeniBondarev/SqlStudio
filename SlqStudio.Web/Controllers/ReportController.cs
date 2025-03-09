using Application.Common.SQL.ResponseModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SlqStudio.Application.CQRS.LabTask.Queries;
using SlqStudio.Application.CQRS.LabWork.Queries;
using SlqStudio.Application.Services.EmailService;
using SlqStudio.DTO;
using SlqStudio.Persistence.Models;
using SlqStudio.Session;

namespace SlqStudio.Controllers;

public class ReportController : Controller
{
    private readonly IMediator _mediator;
    private readonly IEmailService _emailService;

    public ReportController(IMediator mediator, IEmailService emailService)
    {
        _mediator = mediator;
        _emailService = emailService;
    }
    
    public async Task<IActionResult> Index()
    {
        var user = GetUserFromSession();
        var solutionResults = GetSolutionResultsFromSession();
        List<LabWork> labWorks = new List<LabWork>();
        
        foreach (var solutionResult in solutionResults)
        {
            var taskItem = await _mediator.Send(new GetTaskByIdQuery(solutionResult.TaskId));
            if (!labWorks.Any(l => l.Id == taskItem.LabWork.Id))
            {
                labWorks.Add(await _mediator.Send(new GetLabWorkByIdQuery(taskItem.LabWork.Id)));
            }
            
        }
        return View(new ReportDto()
        {
            User = user,
            Solutions = solutionResults,
            LabWorks = labWorks
        });
    }
    
    [HttpPost]
    public async Task<IActionResult> SubmitReport(ReportDto report)
    {
        var result  =await _emailService.SendEmailAsync("evgbondarev@edu.gstu.by", "test", "test");
        return View("ReportSubmitted", report);
    }
    
    public UserDto GetUserFromSession()
    {
        var email = HttpContext.Session.GetString("UserEmail");
        var name = HttpContext.Session.GetString("UserName");

        return new UserDto
        {
            Email = email,
            Name = name
        };
    }
    
    public List<SolutionResultDto> GetSolutionResultsFromSession()
    {
        var comparisonResults = HttpContext.Session.GetObjectFromJson<List<ComparisonResult>>("ComparisonResults") ?? new List<ComparisonResult>();

        return comparisonResults.Select(r => new SolutionResultDto
        {
            TaskId = r.TaskId,
            IsSuccess = r.Result,
            UserSolutionText = r.UserSolutionText
        }).ToList();
    }

}