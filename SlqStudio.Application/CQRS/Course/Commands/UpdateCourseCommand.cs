using MediatR;

namespace SlqStudio.Application.CQRS.Course.Commands;

public record UpdateCourseCommand(int Id, string Name, string Description) : IRequest;