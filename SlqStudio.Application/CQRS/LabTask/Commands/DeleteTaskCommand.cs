using MediatR;

namespace SlqStudio.Application.CQRS.LabTask.Commands;

public record DeleteTaskCommand(int Id) : IRequest;