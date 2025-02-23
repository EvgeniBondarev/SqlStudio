using MediatR;
using Microsoft.AspNetCore.Mvc;
using SlqStudio.Application.CQRS.Course.Commands;
using SlqStudio.Application.CQRS.Course.Commands.Handlers;
using SlqStudio.Application.CQRS.Course.Queries;

namespace SlqStudio.Controllers;

public class CoursesController : Controller
    {
        private readonly IMediator _mediator;
        public CoursesController(IMediator mediator) => _mediator = mediator;

        // GET: /Courses
        public async Task<IActionResult> Index()
        {
            var courses = await _mediator.Send(new GetAllCoursesQuery());
            return View(courses);
        }

        // GET: /Courses/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var course = await _mediator.Send(new GetCourseByIdQuery(id));
            if (course == null)
                return NotFound();
            return View(course);
        }

        // GET: /Courses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCourseCommand command)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(command);
                return RedirectToAction(nameof(Index));
            }
            return View(command);
        }

        // GET: /Courses/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _mediator.Send(new GetCourseByIdQuery(id));
            if (course == null)
                return NotFound();

            // Формируем команду для редактирования с заполненными данными
            var command = new UpdateCourseCommand(course.Id, course.Name, course.Description);
            return View(command);
        }

        // POST: /Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCourseCommand command)
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

        // GET: /Courses/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _mediator.Send(new GetCourseByIdQuery(id));
            if (course == null)
                return NotFound();
            return View(course);
        }

        // POST: /Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _mediator.Send(new DeleteCourseCommand(id));
            return RedirectToAction(nameof(Index));
        }
    }