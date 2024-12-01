using Microsoft.Extensions.Options;
using Quartz;

namespace OnCallSLA.BackgroundJobs;

public class BackgroundJobSetup : IConfigureOptions<QuartzOptions>
{
    private readonly int _interval;

    public BackgroundJobSetup(IConfiguration configuration)
    {
        _interval = configuration.GetRequiredSection("Quartz").GetValue<int>("ScrapeInterval");
    }
    
    public void Configure(QuartzOptions options)
    {
        var jobKey = JobKey.Create(nameof(SliExporterJob));
        
        options
            .AddJob<SliExporterJob>(jobBuilder => jobBuilder
                .WithIdentity(jobKey))
            .AddTrigger(trigger => trigger
                .ForJob(jobKey)
                .WithIdentity("user_sli_job", "sla")
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(_interval)
                    .RepeatForever())
                .StartNow()
            );
    }
}