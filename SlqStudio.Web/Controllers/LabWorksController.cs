using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SlqStudio.Application.CQRS.Course.Queries;
using SlqStudio.Application.CQRS.LabWork.Commands;
using SlqStudio.Application.CQRS.LabWork.Queries;

namespace SlqStudio.Controllers;

public class LabWorksController : Controller
    {
        private readonly IMediator _mediator;
        public LabWorksController(IMediator mediator) => _mediator = mediator;

        // GET: /LabWorks
        public async Task<IActionResult> Index()
        {
            var labWorks = await _mediator.Send(new GetAllLabWorksQuery());
            return View(labWorks);
        }
        
        public async Task<IActionResult> LabList()
        {
            var labWorks = await _mediator.Send(new GetAllLabWorksQuery());
            return View(labWorks);
        }

        // GET: /LabWorks/Details/5
        [Authorize(Roles = "editingteacher")]
        public async Task<IActionResult> Details(int id)
        {
            var labWork = await _mediator.Send(new GetLabWorkByIdQuery(id));
            if (labWork == null)
                return NotFound();
            return View(labWork);
        }

        // GET: /LabWorks/Create
        [Authorize(Roles = "editingteacher")]
        public async Task<IActionResult> Create()
        {
            // Получаем список курсов для dropdown
            var courses = await _mediator.Send(new GetAllCoursesQuery());
            ViewBag.Courses = new SelectList(courses, "Id", "Name");
            return View();
        }

        // POST: /LabWorks/Create
        [HttpPost]
        [Authorize(Roles = "editingteacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateLabWorkCommand command)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(command);
                return RedirectToAction(nameof(Index));
            }
            // Если модель не прошла валидацию – снова подгружаем список курсов
            var courses = await _mediator.Send(new GetAllCoursesQuery());
            ViewBag.Courses = new SelectList(courses, "Id", "Name");
            return View(command);
        }

        // GET: /LabWorks/Edit/5
        [Authorize(Roles = "editingteacher")]
        public async Task<IActionResult> Edit(int id)
        {
            var labWork = await _mediator.Send(new GetLabWorkByIdQuery(id));
            if (labWork == null)
                return NotFound();
            
            // Если модель не прошла валидацию – снова подгружаем список курсов
            var courses = await _mediator.Send(new GetAllCoursesQuery());
            ViewBag.Courses = new SelectList(courses, "Id", "Name");

            var command = new UpdateLabWorkCommand(labWork.Id, labWork.Name, labWork.Description, labWork.Number, labWork.CourseId);
            return View(command);
        }

        // POST: /LabWorks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "editingteacher")]
        public async Task<IActionResult> Edit(int id, UpdateLabWorkCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _mediator.Send(command);
                return RedirectToAction(nameof(Index));
            }
            return View(command);
        }

        // GET: /LabWorks/Delete/5
        [Authorize(Roles = "editingteacher")]
        public async Task<IActionResult> Delete(int id)
        {
            var labWork = await _mediator.Send(new GetLabWorkByIdQuery(id));
            if (labWork == null)
                return NotFound();
            return View(labWork);
        }

        // POST: /LabWorks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "editingteacher")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _mediator.Send(new DeleteLabWorkCommand(id));
            return RedirectToAction(nameof(Index));
        }
    }