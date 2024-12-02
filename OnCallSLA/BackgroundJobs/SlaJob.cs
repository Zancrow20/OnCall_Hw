using OnCallSLA.DataAccess;
using Prometheus;
using Quartz;

namespace OnCallSLA.BackgroundJobs;

[DisallowConcurrentExecution]
public class SlaJob(SliRepository repository, ILogger<SliExporterJob> logger) : IJob
{
    private const string ScenarioName = "prober_create_user_scenario";
    private const string SuccessTotalName = $"{ScenarioName}_success_total";
    private const string TotalName = $"{ScenarioName}_total";
    
    private static readonly Gauge OnCallAvailability = Metrics
        .CreateGauge("oncall_availability_sla", 
            "Gauge of oncall application availability",
    "oncall_sla");
    public async Task Execute(IJobExecutionContext context)
    {
        var response = await repository.Get(context.CancellationToken, context.FireTimeUtc.AddSeconds(-61));

        var success = response
            .Where(x => x.Name is SuccessTotalName)
            .MaxBy(x => x.DateTime);
        
        var total = response
            .Where(x => x.Name is TotalName)
            .MaxBy(x => x.DateTime);

        var availability = success.Value / total.Value;
        OnCallAvailability.Set(availability);
        
        logger.LogInformation("Oncall availability sla: {sla}", availability);
    }
}