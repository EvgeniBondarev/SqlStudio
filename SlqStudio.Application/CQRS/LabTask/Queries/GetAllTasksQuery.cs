using MediatR;

namespace SlqStudio.Application.CQRS.LabTask.Queries;

public class GetAllTasksQuery : IRequest<List<Persistence.Models.LabTask>> { }