using Microsoft.Extensions.Options;
using OnCallProber.Configs;
using OnCallProber.JobsSetup;
using OnCallProber.Probes;
using OnCallProber.Services;
using Prometheus;
using Quartz;

Metrics.SuppressDefaultMetrics();

var builder = WebApplication.CreateBuilder(args);

var url = builder.Configuration.GetValue<string>("ONCALL_EXPORTER_API_URL") ?? "http://oncall.local";

builder.Services.Configure<OnCallExporterConfiguration>(config =>
{
    config.ApiUrl = url;
    config.ScrapeInterval = builder.Configuration.GetValue<int>("ONCALL_EXPORTER_SCRAPE_INTERVAL");

    //todo: https://github.com/linkedin/oncall/issues/262
    config.AppKey = builder.Configuration.GetValue<string>("ONCALL_EXPORTER_APP_KEY");
    config.AppName = builder.Configuration.GetValue<string>("ONCALL_EXPORTER_APP_NAME");
});

builder.Services
    .AddEndpointsApiExplorer()
    .AddQuartz();

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

builder.Services
    .ConfigureOptions<ProbeBackgroundJobSetup>()
    .AddScoped<OnCallTeamService>()
    .AddSingleton<IDefaultMetricsExporter, TeamMetricsExporter>()
    .AddScoped<AuthorizationHeaderService>()
    .AddScoped<SignatureEncoder>(x =>
    {
        var config = x.GetRequiredService<IOptions<OnCallExporterConfiguration>>().Value;
        return new SignatureEncoder(config.AppKey ?? "");
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient("OnCallProberClient", config =>
{
    config.BaseAddress = new Uri(url);
    config.Timeout = new TimeSpan(0, 0, 30);
});

var app = builder.Build();

app.UseMetricServer();

app.UseHttpsRedirection();

await app.RunAsync();