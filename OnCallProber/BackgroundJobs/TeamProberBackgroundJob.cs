using Microsoft.Extensions.Options;
using OnCallProber.Configs;
using OnCallProber.Probes;
using OnCallProber.Services;
using Quartz;

namespace OnCallProber.BackgroundJobs;

[DisallowConcurrentExecution]
public class TeamProberBackgroundJob : IJob
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<TeamProberBackgroundJob> _logger;
    private int _iterator = 0;
    
    public TeamProberBackgroundJob(IServiceScopeFactory serviceScopeFactory, 
        ILogger<TeamProberBackgroundJob> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var teamMetricsService = scope.ServiceProvider.GetRequiredService<OnCallTeamService>();
        var teamMetricsExporter = scope.ServiceProvider.GetRequiredService<IDefaultMetricsExporter>();
        
        var team = new Team(
            $"test-{_iterator++}", 
            "US/Pacific", 
            "team-foo@example.com", 
            $"#team-foo");
        _logger.LogDebug("Attempt to create a new team {@Team}", team);
        teamMetricsExporter.IncreaseProbeTotal();

        using var timer = teamMetricsExporter.NewTimer();
        var result = await teamMetricsService.CreateTeam(team);
        _logger.LogDebug("Duration of attempt to create a new team = {Duration}", timer.ObserveDuration());
        
        if (result)
        {
            _logger.LogDebug("Created new team {@Team}.", team);
            teamMetricsExporter.IncreaseProbeSuccess();
        }
        else
        {
            _logger.LogDebug("Failed to create new team {@Team}.", team);
            teamMetricsExporter.IncreaseProbeFailure();
        }
    }
}