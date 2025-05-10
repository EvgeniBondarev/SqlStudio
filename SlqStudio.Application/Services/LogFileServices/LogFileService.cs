using SlqStudio.Application.Services.LogFileServices.DTO;

namespace SlqStudio.Application.Services.LogFileServices;

public class LogFileService : ILogFileService
{
    private readonly string _logDirectory;

    public LogFileService()
    {
        _logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "logs");
        EnsureLogDirectoryExists();
    }

    private void EnsureLogDirectoryExists()
    {
        if (!Directory.Exists(_logDirectory))
            Directory.CreateDirectory(_logDirectory);
    }

    public LogFileListDto GetLogFiles(int page, int pageSize)
    {
        var allFiles = Directory.GetFiles(_logDirectory)
            .Select(file => new FileInfo(file))
            .OrderByDescending(f => f.LastWriteTime)
            .ToList();

        var totalFiles = allFiles.Count;
        var totalPages = (int)Math.Ceiling(totalFiles / (double)pageSize);
        var filesToShow = allFiles.Skip((page - 1) * pageSize).Take(pageSize);

        return new LogFileListDto
        {
            Files = filesToShow.Select(f => new LogFileDto
            {
                FileName = f.Name,
                FileSize = f.Length,
                LastModified = f.LastWriteTime
            }).ToList(),
            CurrentPage = page,
            TotalPages = totalPages
        };
    }

    public string GetFileContent(string fileName)
    {
        var filePath = Path.Combine(_logDirectory, fileName);
        return System.IO.File.ReadAllText(filePath);
    }

    public byte[] GetFileBytes(string fileName)
    {
        var filePath = Path.Combine(_logDirectory, fileName);
        return System.IO.File.ReadAllBytes(filePath);
    }

    public void DeleteFile(string fileName)
    {
        var filePath = Path.Combine(_logDirectory, fileName);
        System.IO.File.Delete(filePath);
    }
}