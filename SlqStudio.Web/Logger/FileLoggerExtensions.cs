namespace SlqStudio.Logger;

public static class FileLoggerExtensions
{
    public static ILoggingBuilder AddFile(
        this ILoggingBuilder builder,
        IConfiguration configuration)
    {
        var basePath = configuration.GetValue<string>("Path") ?? "logs/app.log";
        var fullPath = Path.IsPathRooted(basePath) 
            ? basePath 
            : Path.Combine(Directory.GetCurrentDirectory(), basePath);

        var maxSizeMb = configuration.GetValue<long>("MaxFileSizeMB", 10);
        var maxFiles = configuration.GetValue<int>("MaxFiles", 5);
        
        RemoveExistingFileProviders(builder);

        return builder.AddProvider(new FileLoggerProvider(
            fullPath,
            maxSizeMb * 1024 * 1024,
            maxFiles));
    }

    private static void RemoveExistingFileProviders(ILoggingBuilder builder)
    {
        var services = builder.Services;
        var descriptors = services.Where(descriptor => 
            descriptor.ImplementationType == typeof(FileLoggerProvider)).ToList();

        foreach (var descriptor in descriptors)
        {
            services.Remove(descriptor);
        }
    }
}