using Prometheus;
using ITimer = Prometheus.ITimer;

namespace OnCallProber.Probes;

public class TeamMetricsExporter : IDefaultMetricsExporter
{
    private const string CreateTeamScenarioName = "prober_create_team_scenario";
    private const string Label = "oncall.prober";
    
    private static readonly Counter ProberCreateTeamScenarioTotal =
        Metrics.CreateCounter($"{CreateTeamScenarioName}_total",
            "Total count of runs the create team scenario to oncall API",
            Label);
    
    private static readonly Counter ProberCreateTeamScenarioSuccessTotal =
        Metrics.CreateCounter($"{CreateTeamScenarioName}_success_total",
            "Total count of success runs the create team scenario to oncall API",
            Label);
    
    private static readonly Counter ProberCreateTeamScenarioFailureTotal =
        Metrics.CreateCounter($"{CreateTeamScenarioName}_fail",
            "Total count of failed runs the create team scenario to oncall API",
            Label);
    
    private static readonly Gauge ProberCreateTeamScenarioDurationSeconds =
        Metrics.CreateGauge($"{CreateTeamScenarioName}_duration_seconds",
            "Duration in seconds of runs the create team scenario to oncall API",
            Label);

    public void IncreaseProbeTotal() => ProberCreateTeamScenarioTotal.Inc();

    public void IncreaseProbeSuccess() => ProberCreateTeamScenarioSuccessTotal.Inc();

    public void IncreaseProbeFailure() => ProberCreateTeamScenarioFailureTotal.Inc();

    public ITimer NewTimer() => ProberCreateTeamScenarioDurationSeconds.NewTimer();
}