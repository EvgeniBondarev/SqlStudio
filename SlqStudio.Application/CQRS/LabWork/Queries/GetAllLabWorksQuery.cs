using MediatR;

namespace SlqStudio.Application.CQRS.LabWork.Queries;

public class GetAllLabWorksQuery : IRequest<List<Persistence.Models.LabWork>> { }