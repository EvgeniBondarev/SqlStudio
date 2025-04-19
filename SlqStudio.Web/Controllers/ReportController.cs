using Application.Common.SQL.ResponseModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SlqStudio.Application.CQRS.LabTask.Queries;
using SlqStudio.Application.CQRS.LabWork.Queries;
using SlqStudio.Application.Services.EmailService;
using SlqStudio.Application.Services.ReportBuider;
using SlqStudio.Application.Services.VariantServices;
using SlqStudio.DTO;
using SlqStudio.Persistence.Models;
using SlqStudio.Session;

namespace SlqStudio.Controllers;

public class ReportController : Controller
{
    private readonly IMediator _mediator;
    private readonly IEmailService _emailService;
    private readonly VariantServices _variantServices;

    public ReportController(IMediator mediator, 
                            IEmailService emailService,
                            VariantServices variantServices)
    {
        _mediator = mediator;
        _emailService = emailService;
        _variantServices = variantServices;
    }

    public async Task<IActionResult> Index()
    {
        var reportDto = await CreateReportDtoAsync();
        return View(reportDto);
    }
    
    [HttpPost]
    public async Task<IActionResult> SubmitReport(ReportDto report)
    {
        var reportDto = await CreateReportDtoAsync();
        var director = new ReportDirector();
        string html = director.BuildHtmlReport(reportDto.User, reportDto.Solutions, reportDto.LabWorks);
        byte[] pdf = director.BuildPdfReport(reportDto.User, reportDto.Solutions, reportDto.LabWorks);
        var result = await _emailService.SendEmailAsync("evgbondarev@edu.gstu.by", reportDto.User.Email, html);
        return File(pdf, "application/pdf", "report.pdf");
    }

    private async Task<ReportDto> CreateReportDtoAsync()
    {
        var user = GetUserFromSession();
        var solutionResults = GetSolutionResultsFromSession();
        var variant = _variantServices.GetVariantFromCache(user.Email);
        
        var labWorks = new List<LabWork>();

        foreach (var solutionResult in solutionResults)
        {
            var taskItem = await _mediator.Send(new GetTaskByIdQuery(solutionResult.TaskId));
            if (!labWorks.Any(l => l.Id == taskItem.LabWork.Id))
            {
                var labWork = await _mediator.Send(new GetLabWorkByIdQuery(taskItem.LabWork.Id));
                labWork.Tasks = labWork.Tasks
                                        .Where(task => variant.Any(v => v.Id == task.Id))
                                        .ToList();
                labWorks.Add(labWork);
            }
        }

        return new ReportDto
        {
            User = user,
            Solutions = solutionResults,
            LabWorks = labWorks
        };
    }

    private UserDto GetUserFromSession()
    {
        var email = HttpContext.Session.GetString("UserEmail");
        var name = HttpContext.Session.GetString("UserName");

        return new UserDto
        {
            Email = email,
            Name = name
        };
    }

    private List<SolutionResultDto> GetSolutionResultsFromSession()
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
