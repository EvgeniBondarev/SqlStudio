using MediatR;

namespace SlqStudio.Application.CQRS.LabTask.Commands;

public record UpdateTaskCommand(int Id, int Number, string Title, string Condition, string SolutionExample) : IRequest;