using SlqStudio.Application.ApiClients.Moodle;
using SlqStudio.Application.ApiClients.Moodle.Models;

namespace SlqStudio.Extensions;

public static class MoodleExtensions
{
    public static void AddMoodleServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MoodleApiSettings>(configuration.GetSection("MoodleApi"));
        services.AddSingleton<MoodleApiClient>();
        services.AddScoped<IMoodleService, MoodleService>();
    }
}