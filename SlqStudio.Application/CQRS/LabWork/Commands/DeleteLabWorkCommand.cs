using MediatR;

namespace SlqStudio.Application.CQRS.LabWork.Commands;

public record DeleteLabWorkCommand(int Id) : IRequest;