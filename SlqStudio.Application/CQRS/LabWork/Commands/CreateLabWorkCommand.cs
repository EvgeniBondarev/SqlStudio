using MediatR;

namespace SlqStudio.Application.CQRS.LabWork.Commands;

public record CreateLabWorkCommand(string Name, string Description, int Number, int CourseId) : IRequest<int>;