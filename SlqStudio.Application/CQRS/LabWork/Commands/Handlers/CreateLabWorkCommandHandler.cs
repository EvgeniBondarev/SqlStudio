using MediatR;
using SlqStudio.Persistence;

namespace SlqStudio.Application.CQRS.LabWork.Commands.Handlers;

public class CreateLabWorkCommandHandler : IRequestHandler<CreateLabWorkCommand, int>
{
    private readonly ApplicationDbContext _context;
    public CreateLabWorkCommandHandler(ApplicationDbContext context) => _context = context;

    public async Task<int> Handle(CreateLabWorkCommand request, CancellationToken ct)
    {
        var labWork = new Persistence.Models.LabWork 
        { 
            Name = request.Name, 
            Description = request.Description, 
            Number = request.Number, 
            CourseId = request.CourseId 
        };

        _context.LabWorks.Add(labWork);
        await _context.SaveChangesAsync(ct);
        return labWork.Id;
    }
}