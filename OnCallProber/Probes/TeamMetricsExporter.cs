using Microsoft.Extensions.Options;
using OnCallProber.Configs;
using Prometheus;
using ITimer = Prometheus.ITimer;

namespace OnCallProber.Probes;

public class TeamMetricsExporter : IDefaultMetricsExporter
{
    private const string CreateTeamScenarioName = "prober_create_team_scenario";

    public TeamMetricsExporter(IOptions<OnCallExporterConfiguration> options)
    {
        var label = options.Value.AppName ?? "";
        ProberCreateTeamScenarioTotal = 
            Metrics.CreateCounter($"{CreateTeamScenarioName}_total",
            "Total count of runs the create team scenario to oncall API",
            label);
        
        ProberCreateTeamScenarioSuccessTotal =
            Metrics.CreateCounter($"{CreateTeamScenarioName}_success_total",
                "Total count of success runs the create team scenario to oncall API",
                label);
        
        ProberCreateTeamScenarioFailureTotal =
            Metrics.CreateCounter($"{CreateTeamScenarioName}_fail_total",
                "Total count of failed runs the create team scenario to oncall API",
                label);
        
        ProberCreateTeamScenarioDurationSeconds =
            Metrics.CreateHistogram($"{CreateTeamScenarioName}_duration_seconds",
                "Duration in seconds of runs the create team scenario to oncall API",
                label);
    }

    private Counter ProberCreateTeamScenarioTotal { get; set; }
        
    
    private Counter ProberCreateTeamScenarioSuccessTotal { get; set; }
    
    private Counter ProberCreateTeamScenarioFailureTotal { get; set; }
    
    private Histogram ProberCreateTeamScenarioDurationSeconds { get; set; }

    public void IncreaseProbeTotal() => ProberCreateTeamScenarioTotal.Inc();

    public void IncreaseProbeSuccess() => ProberCreateTeamScenarioSuccessTotal.Inc();

    public void IncreaseProbeFailure() => ProberCreateTeamScenarioFailureTotal.Inc();

    public ITimer NewTimer() => ProberCreateTeamScenarioDurationSeconds.NewTimer();
}