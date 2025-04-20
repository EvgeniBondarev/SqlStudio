using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SlqStudio.Application.CQRS.Course.Queries;
using SlqStudio.Application.CQRS.LabWork.Commands;
using SlqStudio.Application.CQRS.LabWork.Queries;

namespace SlqStudio.Controllers;

public class LabWorksController : BaseMvcController
{
    private readonly IMediator _mediator;

    public LabWorksController(ILogger<LabWorksController> logger, IMediator mediator)
        : base(logger)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        LogInfo("Загрузка списка всех лабораторных работ");
        var labWorks = await _mediator.Send(new GetAllLabWorksQuery());
        return View(labWorks);
    }

    public async Task<IActionResult> LabList()
    {
        LogInfo("Загрузка представления LabList со списком лабораторных работ");
        var labWorks = await _mediator.Send(new GetAllLabWorksQuery());
        return View(labWorks);
    }

    [Authorize(Roles = "editingteacher")]
    public async Task<IActionResult> Details(int id)
    {
        LogInfo($"Запрос деталей лабораторной работы ID: {id}");
        var labWork = await _mediator.Send(new GetLabWorkByIdQuery(id));
        if (labWork == null)
        {
            LogWarning($"Лабораторная работа с ID {id} не найдена");
            return NotFound();
        }
        return View(labWork);
    }

    [Authorize(Roles = "editingteacher")]
    public async Task<IActionResult> Create()
    {
        LogInfo("Открытие формы создания лабораторной работы");
        var courses = await _mediator.Send(new GetAllCoursesQuery());
        ViewBag.Courses = new SelectList(courses, "Id", "Name");
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "editingteacher")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateLabWorkCommand command)
    {
        if (ModelState.IsValid)
        {
            LogInfo("Создание новой лабораторной работы", command);
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }

        LogWarning("Модель недействительна при создании лабораторной работы", ModelState);
        var courses = await _mediator.Send(new GetAllCoursesQuery());
        ViewBag.Courses = new SelectList(courses, "Id", "Name");
        return View(command);
    }

    [Authorize(Roles = "editingteacher")]
    public async Task<IActionResult> Edit(int id)
    {
        LogInfo($"Открытие формы редактирования лабораторной работы ID: {id}");
        var labWork = await _mediator.Send(new GetLabWorkByIdQuery(id));
        if (labWork == null)
        {
            LogWarning($"Лабораторная работа с ID {id} не найдена для редактирования");
            return NotFound();
        }

        var courses = await _mediator.Send(new GetAllCoursesQuery());
        ViewBag.Courses = new SelectList(courses, "Id", "Name");

        var command = new UpdateLabWorkCommand(labWork.Id, labWork.Name, labWork.Description, labWork.Number, labWork.CourseId);
        return View(command);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "editingteacher")]
    public async Task<IActionResult> Edit(int id, UpdateLabWorkCommand command)
    {
        if (id != command.Id)
        {
            LogWarning($"Несовпадение ID при редактировании: путь ID = {id}, тело ID = {command.Id}");
            return BadRequest();
        }

        if (ModelState.IsValid)
        {
            LogInfo($"Обновление лабораторной работы ID: {id}", command);
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }

        LogWarning("Модель недействительна при редактировании лабораторной работы", ModelState);
        var courses = await _mediator.Send(new GetAllCoursesQuery());
        ViewBag.Courses = new SelectList(courses, "Id", "Name");
        return View(command);
    }

    [Authorize(Roles = "editingteacher")]
    public async Task<IActionResult> Delete(int id)
    {
        LogInfo($"Запрос на удаление лабораторной работы ID: {id}");
        var labWork = await _mediator.Send(new GetLabWorkByIdQuery(id));
        if (labWork == null)
        {
            LogWarning($"Лабораторная работа с ID {id} не найдена для удаления");
            return NotFound();
        }
        return View(labWork);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "editingteacher")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        LogInfo($"Подтверждено удаление лабораторной работы ID: {id}");
        await _mediator.Send(new DeleteLabWorkCommand(id));
        return RedirectToAction(nameof(Index));
    }
}
