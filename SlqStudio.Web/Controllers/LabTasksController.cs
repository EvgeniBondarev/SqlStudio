using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SlqStudio.Application.CQRS.LabTask.Commands;
using SlqStudio.Application.CQRS.LabTask.Queries;
using SlqStudio.Application.CQRS.LabWork.Queries;
using SlqStudio.Application.Services.VariantServices;

namespace SlqStudio.Controllers;

//[Authorize(Roles = "editingteacher")]
public class LabTasksController : Controller
{
        private readonly IMediator _mediator;
        private readonly VariantServices _variantServices;
        public LabTasksController(IMediator mediator, VariantServices variantServices)
        {
            _mediator = mediator;
            _variantServices = variantServices;
        }

        // GET: /LabTasks
        public async Task<IActionResult> Index()
        {
            var tasks = await _mediator.Send(new GetAllTasksQuery());
            return View(tasks);
        }
        
        
        [Authorize(Roles = "editingteacher")]
        public async Task<IActionResult> Details(int id)
        {
            var taskItem = await _mediator.Send(new GetTaskByIdQuery(id));
            if (taskItem == null)
                return NotFound();
            return View(taskItem);
        }
        
        public async Task<IActionResult> DetailsByWork(int id)
        {
            var labWorkItem = await _mediator.Send(new GetLabWorkByIdQuery(id));
            if (labWorkItem == null)
                return NotFound();
            return View(_variantServices.GenerateVariant(labWorkItem.Tasks.ToList(), HttpContext.Session.GetString("UserEmail")));
        }

        
        [Authorize(Roles = "editingteacher")]
        public async Task<IActionResult> Create()
        {
            var labWorks = await _mediator.Send(new GetAllLabWorksQuery());
            ViewBag.LabWorks = new SelectList(labWorks, "Id", "Name");
            return View();
        }

        [Authorize(Roles = "editingteacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTaskCommand command)
        {
            if (ModelState.IsValid)
            {
                await _mediator.Send(command);
                return RedirectToAction(nameof(Index));
            }
            var labWorks = await _mediator.Send(new GetAllLabWorksQuery());
            ViewBag.LabWorks = new SelectList(labWorks, "Id", "Name");
            return View(command);
        }

        [Authorize(Roles = "editingteacher")]
        public async Task<IActionResult> Edit(int id)
        {
            var taskItem = await _mediator.Send(new GetTaskByIdQuery(id));
            if (taskItem == null)
                return NotFound();

            var command = new UpdateTaskCommand(taskItem.Id, taskItem.Number, taskItem.Title, taskItem.Condition, taskItem.SolutionExample, taskItem.LabWorkId);
            var labWorks = await _mediator.Send(new GetAllLabWorksQuery());
            ViewBag.LabWorks = new SelectList(labWorks, "Id", "Name", taskItem.LabWorkId);
            return View(command);
        }

        [Authorize(Roles = "editingteacher")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateTaskCommand command)
        {
            if (id != command.Id)
                return BadRequest();

            if (ModelState.IsValid)
            {
                await _mediator.Send(command);
                return RedirectToAction(nameof(Index));
            }
            var labWorks = await _mediator.Send(new GetAllLabWorksQuery());
            ViewBag.LabWorks = new SelectList(labWorks, "Id", "Name");
            return View(command);
        }

        [Authorize(Roles = "editingteacher")]
        public async Task<IActionResult> Delete(int id)
        {
            var taskItem = await _mediator.Send(new GetTaskByIdQuery(id));
            if (taskItem == null)
                return NotFound();
            return View(taskItem);
        }

        [Authorize(Roles = "editingteacher")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _mediator.Send(new DeleteTaskCommand(id));
            return RedirectToAction(nameof(Index));
        }
}