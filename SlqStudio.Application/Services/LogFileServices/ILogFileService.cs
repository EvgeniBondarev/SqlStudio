using SlqStudio.Application.Services.LogFileServices.DTO;

namespace SlqStudio.Application.Services.LogFileServices;

public interface ILogFileService
{
    LogFileListDto GetLogFiles(int page, int pageSize);
    string GetFileContent(string fileName);
    byte[] GetFileBytes(string fileName);
    void DeleteFile(string fileName);
}