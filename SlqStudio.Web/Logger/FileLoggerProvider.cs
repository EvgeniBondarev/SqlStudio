namespace SlqStudio.Logger;

public class FileLoggerProvider : ILoggerProvider
{
    private readonly string _basePath;
    private readonly long _maxFileSize;
    private readonly int _maxFiles;

    public FileLoggerProvider(string basePath, long maxFileSizeBytes, int maxFiles)
    {
        _basePath = basePath;
        _maxFileSize = maxFileSizeBytes;
        _maxFiles = maxFiles;
    }

    public ILogger CreateLogger(string categoryName) => 
        new FileLogger(_basePath, _maxFileSize, _maxFiles);

    public void Dispose() { }
}