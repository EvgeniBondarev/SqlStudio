using Newtonsoft.Json.Linq;

namespace SlqStudio.Application.Services.AppSettingsServices;

public class AppSettingsService : IAppSettingsService
{
    private readonly string _commentsPath;

    public AppSettingsService()
    {
        _commentsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings-comments.json");
    }

    public JObject ReadConfig(string configPath)
    {
        var json = File.ReadAllText(configPath);
        return JObject.Parse(json);
    }

    public JObject ReadComments()
    {
        if (!File.Exists(_commentsPath))
            return new JObject();
            
        var json = File.ReadAllText(_commentsPath);
        return JObject.Parse(json);
    }

    public void WriteConfig(JObject config, string configPath)
    {
        var updatedJson = config.ToString();
        File.WriteAllText(configPath, updatedJson);
    }
}