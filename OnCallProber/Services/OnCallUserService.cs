using System.Net;

namespace OnCallProber.Services;

public class OnCallUserService
{
    private readonly ILogger<OnCallUserService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private const string Endpoint = "/api/v0/users"; 
    public OnCallUserService(IHttpClientFactory httpClientFactory, 
        ILogger<OnCallUserService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<bool> CreateUser(string userName, CancellationToken cancellationToken)
    {
        try
        {
            using var httpClient = _httpClientFactory.CreateClient("OnCallProberClient");
            var response = await httpClient.PostAsJsonAsync(Endpoint, new { name = userName },
                cancellationToken: cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Remote returned invalid status code: {code}", response.StatusCode);
                return false;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create user: {@User} creation due to exception.", userName);
            return false;
        }

        return true;
    }

    public async Task<bool> DeleteUser(string userName, CancellationToken cancellationToken)
    {
        var endpoint = $"{Endpoint}/{userName}";
        try
        {
            using var httpClient = _httpClientFactory.CreateClient("OnCallProberClient");
            var response = await httpClient.DeleteAsync(endpoint,
                cancellationToken: cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Remote returned invalid status code: {code}", response.StatusCode);
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    _logger.LogInformation(
                        "User {User} was not found in remote while delete operation.", 
                        userName);
                }
                return false;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete user: {@User} creation due to exception.", userName);
            return false;
        }

        return true;
    }
}