namespace SlqStudio.Application.Services.LogFileServices.DTO;

public class LogFileDto
{
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public DateTime LastModified { get; set; }
}