using MediatR;
using Microsoft.EntityFrameworkCore;
using SlqStudio.Persistence;

namespace SlqStudio.Application.CQRS.LabWork.Queries.Handlers;

public class GetAllLabWorksQueryHandler : IRequestHandler<GetAllLabWorksQuery, List<Persistence.Models.LabWork>>
{
    private readonly ApplicationDbContext _context;
    public GetAllLabWorksQueryHandler(ApplicationDbContext context) => _context = context;

    public async Task<List<Persistence.Models.LabWork>> Handle(GetAllLabWorksQuery request, CancellationToken ct) 
        => await _context.LabWorks
            .Include(lw => lw.Course)
            .Include(lw => lw.Tasks)
            .ToListAsync(ct);
}
