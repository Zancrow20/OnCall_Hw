﻿using System.Globalization;
using OnCallSLA.DataAccess;
using OnCallSLA.Models;
using OnCallSLA.Services;
using Quartz;

namespace OnCallSLA.BackgroundJobs;

[DisallowConcurrentExecution]
public class SliExporterJob(SliRepository repository, ILogger<SliExporterJob> logger, PrometheusClient prometheusClient) : IJob
{
    private const string Scenario = "prober_create_user_scenario";
    private const string SuccessTotalName = $"{Scenario}_success_total";
    private const string TotalName = $"{Scenario}_total";
    private const string DurationName = $"{Scenario}_duration_seconds";

    public async Task Execute(IJobExecutionContext context)
    {
        await SaveSuccessTotal(context);
        await SaveTotal(context);
        await SaveDuration(context);
    }

    public async Task SaveSuccessTotal(IJobExecutionContext context)
    {
        var time = context.FireTimeUtc;
        
        var total = await prometheusClient.GetLastValue(
            $"increase({SuccessTotalName}[1m])", 
            time, 
            defaultValue: "0");

        if (!float.TryParse(total, CultureInfo.InvariantCulture, out var value))
        {
            logger.LogError("Failed to parse sli value from prometheus:{ScrapeName}:{invalidValue}:{TargetClrType}",
                nameof(CreateUserSli), 
                total,
                "float"
            );
            return;
        }
        
        var sli = new CreateUserSli(time, SuccessTotalName, 1, value, value < 1);

        await repository.Add(sli, context.CancellationToken);
    }

    public async Task SaveTotal(IJobExecutionContext context)
    {
        var time = context.FireTimeUtc;
        
        var total = await prometheusClient.GetLastValue(
            $"increase({TotalName}[1m])", 
            time, 
            defaultValue: "0");

        if (!float.TryParse(total, CultureInfo.InvariantCulture, out var value))
        {
            logger.LogError("Failed to parse sli value from prometheus:{ScrapeName}:{invalidValue}:{TargetClrType}",
                nameof(CreateUserSli), 
                total,
                "float"
            );
            return;
        }
        
        var sli = new CreateUserSli(time, TotalName, 1, value, value < 1);

        await repository.Add(sli, context.CancellationToken);
    }
    
    public async Task SaveDuration(IJobExecutionContext context)
    {
        var time = context.FireTimeUtc;
        
        var duration = await prometheusClient.GetLastValue(
            DurationName, 
            time, 
            defaultValue: "2");

        if (!float.TryParse(duration, CultureInfo.InvariantCulture, out var value))
        {
            logger.LogError("Failed to parse sli value from prometheus:{ScrapeName}:{invalidValue}:{TargetClrType}",
                nameof(CreateUserSli), 
                duration,
                "float"
            );
            return;
        }
        
        var sli = new CreateUserSli(time, DurationName, 0.1f, value, value > 0.1f);

        await repository.Add(sli, context.CancellationToken);
    }
}