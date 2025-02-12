using MediatR;

namespace SlqStudio.Application.CQRS.LabWork.Commands;

public record UpdateLabWorkCommand(int Id, string Name, string Description, int Number) : IRequest;