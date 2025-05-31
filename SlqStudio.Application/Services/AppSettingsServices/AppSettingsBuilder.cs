using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;

namespace SlqStudio.Application.Services.AppSettingsServices;

public class AppSettingsBuilder
{
    public JObject BuildAppSettings(IFormCollection form)
    {
        var updatedConfig = new JObject();
        var comments = new JObject();

        foreach (var item in form)
        {
            if (item.Key == "__RequestVerificationToken")
                continue;

            // Сохраняем комментарии отдельно
            if (item.Key.StartsWith("Comments."))
            {
                var commentKey = item.Key.Substring("Comments.".Length);
                comments[commentKey] = item.Value.ToString();
                continue;
            }

            var keys = item.Key.Split('.');
            JToken current = updatedConfig;

            for (int i = 0; i < keys.Length; i++)
            {
                var key = keys[i];

                if (i == keys.Length - 1)
                {
                    ((JObject)current)[key] = item.Value.ToString();
                }
                else
                {
                    if (current[key] is JObject next)
                    {
                        current = next;
                    }
                    else
                    {
                        var newObj = new JObject();
                        ((JObject)current)[key] = newObj;
                        current = newObj;
                    }
                }
            }
        }
        
        if (comments.HasValues)
        {
            updatedConfig["Comments"] = comments;
        }

        return updatedConfig;
    }
}