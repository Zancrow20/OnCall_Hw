using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace OnCallProber.Services;

public class OnCallTeamService
{
    private readonly ILogger<OnCallTeamService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthorizationHeaderService _authorizationHeaderService;
    private const string Endpoint = "/api/v0/teams"; 
    public OnCallTeamService(IHttpClientFactory httpClientFactory, 
        ILogger<OnCallTeamService> logger,
        AuthorizationHeaderService authorizationHeaderService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizationHeaderService = authorizationHeaderService;
    }

    public async Task<bool> CreateTeam(Team team)
    {
        var json = JsonSerializer.Serialize(team);
        var jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
        var authHeader = _authorizationHeaderService.GetAuthorizationHeader(Endpoint, HttpMethod.Post, json);

        try
        {
            using var httpClient = _httpClientFactory.CreateClient("OnCallProberClient");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authHeader); 
            var response = await httpClient.PostAsync(Endpoint, jsonContent);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Remote returned invalid status code {code}", response.StatusCode);
                return false;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create {@Team} creation due to exception.", team);
            return false;
        }

        return true;
    }
}

public record Team(
    [property: JsonPropertyName("name")]
    string Name,
    [property: JsonPropertyName("scheduling_timezone")]
    string SchedulingTimezone,
    [property: JsonPropertyName("email")]
    string Email,
    [property: JsonPropertyName("slack_channel")]
    string SlackChannel);