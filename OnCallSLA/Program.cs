using MySqlConnector;
using OnCallSLA.BackgroundJobs;
using OnCallSLA.DataAccess;
using OnCallSLA.Services;
using Prometheus;
using Quartz;

Metrics.SuppressDefaultMetrics();

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddQuartz();

builder.Services.AddQuartzHostedService(options =>
{
    options.WaitForJobsToComplete = true;
});

builder.Services
    .AddLogging(log => log.AddConsole())
    .AddHttpClient()
    .AddTransient<SliRepository>()
    .AddTransient<PrometheusClient>(x => new PrometheusClient(
        builder.Configuration.GetSection("Prometheus").GetValue<string>("Url"), 
        x.GetRequiredService<HttpClient>()))
    .AddMySqlDataSource(builder.Configuration.GetConnectionString("MySql"));

builder.Services
    .AddHostedService<MigrationBackgroundService>()
    .ConfigureOptions<BackgroundJobSetup>();

var app = builder.Build();

app.UseMetricServer();

await app.RunAsync();