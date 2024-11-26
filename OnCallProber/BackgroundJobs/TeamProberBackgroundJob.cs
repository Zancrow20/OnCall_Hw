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
    private readonly OnCallExporterConfiguration _config;
    
    public TeamProberBackgroundJob(IServiceScopeFactory serviceScopeFactory, 
        ILogger<TeamProberBackgroundJob> logger, IOptions<OnCallExporterConfiguration> config)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _config = config.Value;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var teamMetricsService = scope.ServiceProvider.GetRequiredService<OnCallTeamService>();
        var teamMetricsExporter = scope.ServiceProvider.GetRequiredService<IDefaultMetricsExporter>();
        
        var team = new Team("test", "US/Pacific", "team-foo@example.com", "#team-foo");
        _logger.LogDebug("Attempt to create a new team {@Team}", team);
        teamMetricsExporter.IncreaseProbeTotal();

        var timer = teamMetricsExporter.NewTimer();
        var result = await teamMetricsService.CreateTeam(team);
        timer.Dispose();
        
        if (result)
        {
            _logger.LogDebug("Created new team {@Team}.", team);
            teamMetricsExporter.IncreaseProbeSuccess();
        }
        else
        {
            _logger.LogDebug("Failed to create new team {@Event}.", team);
            teamMetricsExporter.IncreaseProbeFailure();
        }
    }
}