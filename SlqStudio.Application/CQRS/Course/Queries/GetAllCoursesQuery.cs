using MediatR;

namespace SlqStudio.Application.CQRS.Course.Queries;

public class GetAllCoursesQuery : IRequest<List<Persistence.Models.Course>> { }