using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;

namespace SlqStudio.Application.ApiClients.Moodle;

public class MoodleApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
    private readonly MoodleApiSettings _settings;

    public MoodleApiClient(IHttpClientFactory httpClientFactory, IOptions<MoodleApiSettings> options)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _settings = options.Value ?? throw new ArgumentNullException(nameof(options));
        
        _retryPolicy = Policy<HttpResponseMessage>
            .Handle<HttpRequestException>()
            .OrResult(response => !response.IsSuccessStatusCode)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2));
    }

    public async Task<List<TResponse>> SendRequestAsync<TResponse>(string functionName, Dictionary<string, string> parameters)
    {
        var client = _httpClientFactory.CreateClient();
        var queryParams = new StringBuilder();
        queryParams.Append($"?wstoken={_settings.Token}");
        queryParams.Append($"&wsfunction={functionName}");
        queryParams.Append($"&moodlewsrestformat=json");
        
        foreach (var param in parameters)
        {
            queryParams.Append($"&{param.Key}={param.Value}");
        }

        HttpResponseMessage response = await _retryPolicy.ExecuteAsync(() => client.GetAsync(_settings.MoodleUrl + queryParams.ToString()));
        response.EnsureSuccessStatusCode();

        string jsonResponse = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<TResponse>>(jsonResponse);
    }
}

public class MoodleCourseResponse
{
    public int Id { get; set; }
    public string Shortname { get; set; }
    public int CategoryId { get; set; }
    public string Fullname { get; set; }
    public string Displayname { get; set; }
    public string Summary { get; set; }
    public int StartDate { get; set; }
    public int EndDate { get; set; }
    public int Visible { get; set; }
}


public class MoodleApiSettings
{
    public string MoodleUrl { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
}

public class MoodleUserProfileRequest
{
    public int UserId { get; set; }
    public int CourseId { get; set; }
}

public class MoodleUserProfileResponse
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string ProfileImageUrl { get; set; }
    public List<MoodleUserRole> Roles { get; set; } = new List<MoodleUserRole>();
    public List<MoodleUserCourse> EnrolledCourses { get; set; } = new List<MoodleUserCourse>();
}

public class MoodleUserRole
{
    public int RoleId { get; set; }
    public string ShortName { get; set; }
    public string Name { get; set; }
}

public class MoodleUserCourse
{
    public int Id { get; set; }
    public string FullName { get; set; }
    public string ShortName { get; set; }
}