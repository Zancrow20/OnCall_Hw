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
        
        ConfigureSecondJob(options);
    }

    private void ConfigureSecondJob(QuartzOptions options)
    {
        var jobKey2 = JobKey.Create(nameof(SlaJob));
        var now = SystemTime.UtcNow();
        
        options
            .AddJob<SlaJob>(jobBuilder => jobBuilder
                .WithIdentity(jobKey2))
            .AddTrigger(trigger => trigger
                .ForJob(jobKey2)
                .WithIdentity("user_sla_job", "sla")
                .WithSimpleSchedule(x => x
                    .WithIntervalInSeconds(_interval)
                    .RepeatForever())
                .StartAt(now.AddSeconds(_interval))
            );
    }
}