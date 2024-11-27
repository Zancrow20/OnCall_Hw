using Microsoft.Extensions.Options;
using OnCallProber.BackgroundJobs;
using Quartz;

namespace OnCallProber.JobsSetup;

public class ProbeBackgroundJobSetup : IConfigureOptions<QuartzOptions>
{
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
                    .WithIntervalInSeconds(10)
                    .RepeatForever())
                .StartNow()
            );
    }
}