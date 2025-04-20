using SlqStudio.Application.Services.AppSettingsServices;
using SlqStudio.Application.Services.EmailService;
using SlqStudio.Application.Services.EmailService.Implementation;
using SlqStudio.Application.Services.EmailService.Models;
using SlqStudio.Application.Services.VariantServices;

namespace SlqStudio.Extensions;

public static class ApplicationServicesExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<VariantServices>();

        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
        services.AddTransient<IEmailService, EmailService>();

        services.AddSingleton<IAppSettingsService, AppSettingsService>();
        services.AddSingleton<AppSettingsBuilder>();
    }
}