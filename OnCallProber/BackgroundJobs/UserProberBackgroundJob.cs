using OnCallProber.Probes;
using OnCallProber.Services;
using Quartz;

namespace OnCallProber.BackgroundJobs;

[DisallowConcurrentExecution]
public class UserProberBackgroundJob : IJob
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<UserProberBackgroundJob> _logger;
    private readonly OnCallUserService _userService;
    private readonly IDefaultMetricsExporter _defaultMetricsExporter;
    
    public UserProberBackgroundJob(IServiceScopeFactory serviceScopeFactory, 
        ILogger<UserProberBackgroundJob> logger,
        OnCallUserService userService,
        IDefaultMetricsExporter defaultMetricsExporter)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _userService = userService;
        _defaultMetricsExporter = defaultMetricsExporter;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var userName = $"test_user_{Random.Shared.Next()}";
        _logger.LogDebug("Attempt to create a new user {user}", userName);

        var result = await TryCreateUser(userName, context.CancellationToken);
        
        if (!result)
        {
            await DeleteUser(userName, context.CancellationToken);
        }
    }

    private async Task<bool> TryCreateUser(string userName, CancellationToken cancellationToken)
    {
        _defaultMetricsExporter.IncreaseProbeTotal();
        using var timer = _defaultMetricsExporter.NewTimer();
        
        var result = await _userService.CreateUser(userName, cancellationToken);
        _logger.LogDebug("Duration of attempt to create a new team = {Duration}", timer.ObserveDuration());
        
        if (result)
        {
            _logger.LogDebug("Created new user {user}.", userName);
            _defaultMetricsExporter.IncreaseProbeSuccess();
        }
        else
        {
            _logger.LogDebug("Failed to create new user {user}.", userName);
            _defaultMetricsExporter.IncreaseProbeFailure();
        }

        return result;
    }
    
    private async Task DeleteUser(string userName, CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteUser(userName, cancellationToken);
        
        if (result)
        {
            _logger.LogDebug("Deleted user: {user}.", userName);
        }
        else
        {
            _logger.LogDebug("Failed to delete user: {user}.", userName);
        }
    }
}