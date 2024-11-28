using Microsoft.Extensions.Options;
using OnCallProber.BackgroundJobs;
using OnCallProber.Configs;
using Quartz;

namespace OnCallProber.JobsSetup;

public class ProbeBackgroundJobSetup : IConfigureOptions<QuartzOptions>
{
    private readonly OnCallExporterConfiguration _config;

    public ProbeBackgroundJobSetup(IOptions<OnCallExporterConfiguration> options)
    {
        _config = options.Value;
    }

    public void Configure(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(TeamProberBackgroundJob));
        
        options
            .AddJob<TeamProberBackgroundJob>(jobBuilder => jobBuilder
                .WithIdentity(jobKey))
            .AddTrigger(trigger => trigger
                .ForJob(jobKey)
                .WithIdentity("team_prober_job", "prober")
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(_config.ScrapeInterval)
                    .RepeatForever())
                .StartNow()
            );
    }
}