using MediatR;

namespace SlqStudio.Application.CQRS.Course.Commands;

public record CreateCourseCommand(string Name, string Description) : IRequest<int>;