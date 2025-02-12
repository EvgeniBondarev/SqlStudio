using MediatR;
using SlqStudio.Persistence;

namespace SlqStudio.Application.CQRS.Course.Commands.Handlers;

public class CreateCourseCommandHandler : IRequestHandler<CreateCourseCommand, int>
{
    private readonly ApplicationDbContext _context;
    public CreateCourseCommandHandler(ApplicationDbContext context) => _context = context;

    public async Task<int> Handle(CreateCourseCommand request, CancellationToken ct)
    {
        var course = new Persistence.Models.Course 
        { 
            Name = request.Name, 
            Description = request.Description 
        };

        _context.Courses.Add(course);
        await _context.SaveChangesAsync(ct);
        return course.Id;
    }
}