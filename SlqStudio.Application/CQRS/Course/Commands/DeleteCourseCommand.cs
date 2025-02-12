using MediatR;

namespace SlqStudio.Application.CQRS.Course.Commands.Handlers;

public record DeleteCourseCommand(int Id) : IRequest;