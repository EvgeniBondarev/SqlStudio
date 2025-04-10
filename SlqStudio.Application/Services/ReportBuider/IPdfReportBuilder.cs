namespace SlqStudio.Application.Services.ReportBuider;

public interface IPdfReportBuilder : IReportBuilder, IDisposable
{
    byte[] Build();
}