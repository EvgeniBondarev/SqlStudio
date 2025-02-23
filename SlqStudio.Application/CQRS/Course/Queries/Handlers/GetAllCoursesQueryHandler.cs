using MediatR;
using Microsoft.EntityFrameworkCore;
using SlqStudio.Persistence;

namespace SlqStudio.Application.CQRS.Course.Queries.Handlers;

public class GetAllCoursesQueryHandler : IRequestHandler<GetAllCoursesQuery, List<Persistence.Models.Course>>
{
    private readonly ApplicationDbContext _context;
    public GetAllCoursesQueryHandler(ApplicationDbContext context) => _context = context;

    public async Task<List<Persistence.Models.Course>> Handle(GetAllCoursesQuery request, CancellationToken ct)
        => await _context.Courses.Include(c => c.LabWorks).ToListAsync(ct);
}