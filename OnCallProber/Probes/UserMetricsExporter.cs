using Microsoft.Extensions.Options;
using OnCallProber.Configs;
using Prometheus;
using ITimer = Prometheus.ITimer;

namespace OnCallProber.Probes;

public class UserMetricsExporter : IDefaultMetricsExporter
{
    private const string CreateUserScenarioName = "prober_create_user_scenario";

    public UserMetricsExporter(IOptions<OnCallExporterConfiguration> options)
    {
        var label = options.Value.AppName ?? "";
        ProberCreateUserScenarioTotal = 
            Metrics.CreateCounter($"{CreateUserScenarioName}_total",
            "Total count of runs the create team scenario to oncall API",
            label);
        
        ProberCreateUserScenarioSuccessTotal =
            Metrics.CreateCounter($"{CreateUserScenarioName}_success_total",
                "Total count of success runs the create team scenario to oncall API",
                label);
        
        ProberCreateUserScenarioFailureTotal =
            Metrics.CreateCounter($"{CreateUserScenarioName}_fail_total",
                "Total count of failed runs the create team scenario to oncall API",
                label);
        
        ProberCreateUserScenarioDurationSeconds =
            Metrics.CreateHistogram($"{CreateUserScenarioName}_duration_seconds",
                "Duration in seconds of runs the create team scenario to oncall API",
                label);
    }

    private Counter ProberCreateUserScenarioTotal { get; set; }
        
    
    private Counter ProberCreateUserScenarioSuccessTotal { get; set; }
    
    private Counter ProberCreateUserScenarioFailureTotal { get; set; }
    
    private Histogram ProberCreateUserScenarioDurationSeconds { get; set; }

    public void IncreaseProbeTotal() => ProberCreateUserScenarioTotal.Inc();

    public void IncreaseProbeSuccess() => ProberCreateUserScenarioSuccessTotal.Inc();

    public void IncreaseProbeFailure() => ProberCreateUserScenarioFailureTotal.Inc();

    public ITimer NewTimer() => ProberCreateUserScenarioDurationSeconds.NewTimer();
}