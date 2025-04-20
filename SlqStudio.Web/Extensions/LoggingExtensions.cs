using SlqStudio.Logger;

namespace SlqStudio.Extensions;

public static class LoggingExtensions
{
    public static void ConfigureLogging(this ILoggingBuilder logging, IConfiguration configuration)
    {
        logging
            .ClearProviders()
            .AddConsole()
            .AddDebug()
            .AddConfiguration(configuration.GetSection("Logging"))
            .AddFile(configuration.GetSection("Logging:File"));
    }
}