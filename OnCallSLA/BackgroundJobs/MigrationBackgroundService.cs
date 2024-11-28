using MySqlConnector;
using OnCallSLA.DataAccess;

namespace OnCallSLA.BackgroundJobs;

public class MigrationBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public MigrationBackgroundService(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var scope = _serviceScopeFactory.CreateScope();
        var mySql = scope.ServiceProvider.GetRequiredService<MySqlConnection>();
        await mySql.OpenAsync(stoppingToken);
        await mySql.BaseMigrate(stoppingToken);
        await mySql.CloseAsync();
    }
}