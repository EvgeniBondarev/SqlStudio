using MediatR;
using SlqStudio.Persistence;

namespace SlqStudio.Application.CQRS.LabTask.Queries.Handler;

public class GetTaskByIdQueryHandler : IRequestHandler<GetTaskByIdQuery, Persistence.Models.LabTask>
{
    private readonly ApplicationDbContext _context;
    public GetTaskByIdQueryHandler(ApplicationDbContext context) => _context = context;

    public async Task<Persistence.Models.LabTask> Handle(GetTaskByIdQuery request, CancellationToken ct)
        => await _context.LabTasks.FindAsync(request.Id);
}