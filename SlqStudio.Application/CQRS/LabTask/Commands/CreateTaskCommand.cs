using MediatR;

namespace SlqStudio.Application.CQRS.LabTask.Commands;

public record CreateTaskCommand(int Number, string Title, string Condition, string SolutionExample, int LabWorkId) : IRequest<int>;