using Application.Common.SQL;
using Microsoft.EntityFrameworkCore;
using SlqStudio.Persistence;

namespace SlqStudio.Extensions;

public static class DatabaseExtensions
{
    public static void AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("MySQL");
        var labsConnectionString = configuration.GetConnectionString("LabsConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        services.AddSingleton<SqlManager>(sp => new SqlManager(labsConnectionString));
    }
}