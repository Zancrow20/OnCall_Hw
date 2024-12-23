using Prometheus;

const string version = "v3", versionLabel = "Version";

const string instanceIdLabel = "InstanceId";
var instanceId = Guid.NewGuid().ToString();

var requestsTotal = Metrics.CreateCounter("requests_total", "Total received requests count", 
    versionLabel, instanceIdLabel);

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () =>
{
    requestsTotal.WithLabels(version, instanceId).Inc();
    return $"Version: {version}\tResponse from {instanceId}\n";
});

app.MapGet("/healthcheck", () => Results.Ok());

app.MapMetrics();

app.Run();