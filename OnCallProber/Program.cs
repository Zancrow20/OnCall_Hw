using Microsoft.Extensions.Options;
using OnCallProber.Configs;
using OnCallProber.JobsSetup;
using OnCallProber.Probes;
using OnCallProber.Services;
using Prometheus;
using Quartz;

Metrics.SuppressDefaultMetrics();

var builder = WebApplication.CreateBuilder(args);

var onCall = builder.Configuration.GetSection("OnCall"); 

builder.Services.Configure<OnCallExporterConfiguration>(onCall);

builder.Services
    .AddEndpointsApiExplorer()
    .AddQuartz();

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

builder.Services
    .AddLogging(log => log.AddConsole())
    .ConfigureOptions<ProbeBackgroundJobSetup>()
    .AddScoped<OnCallUserService>()
    .AddSingleton<IDefaultMetricsExporter, UserMetricsExporter>();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient("OnCallProberClient", config =>
{
    config.BaseAddress = new Uri(onCall.GetValue<string>("ExporterApiUrl") ?? string.Empty);
    config.Timeout = new TimeSpan(0, 0, 30);
});

var app = builder.Build();

app.UseMetricServer();

app.UseHttpsRedirection();

await app.RunAsync();