namespace SlqStudio.Application.Services.DiagramServices;

public interface IDiagramService
{
    Task<string> LoadDiagramAsync(string diagramName);
}
