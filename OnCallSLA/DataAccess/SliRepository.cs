using MySqlConnector;
using OnCallSLA.Models;

namespace OnCallSLA.DataAccess;

public class SliRepository
{
    private readonly MySqlConnection _mySqlConnection;

    public SliRepository(MySqlConnection mySqlConnection)
    {
        _mySqlConnection = mySqlConnection;
    }

    public async Task Add(Sli sli, CancellationToken cancellationToken)
    {
        const string sql = """
                           USE Sla;
                           INSERT INTO `sli`(datetime, name, slo, value, isBad) VALUES (@DateTime, @Name, @Slo, @Value, @IsBad);
                           """;
        var command = new MySqlCommand(sql, _mySqlConnection);
        command.Parameters.AddWithValue("@DateTime", sli.DateTime);
        command.Parameters.AddWithValue("@Name", sli.Name);
        command.Parameters.AddWithValue("@Slo", sli.Slo);
        command.Parameters.AddWithValue("@Value", sli.Value);
        command.Parameters.AddWithValue("@IsBad", sli.IsBad);
        
        await _mySqlConnection.OpenAsync(cancellationToken);
        await command.ExecuteNonQueryAsync(cancellationToken);
        await _mySqlConnection.CloseAsync();
    }
}

public static class DbMigrations
{
    public static async Task BaseMigrate(this MySqlConnection conn, CancellationToken cancellationToken)
    {
        const string sql = """
                           CREATE DATABASE IF NOT EXISTS sla;
                           USE sla;
                           CREATE TABLE IF NOT EXISTS `sli`(
                               datetime datetime not null default NOW(),
                               name varchar(255) not null,
                               slo float(4) not null,
                               value float(4) not null,
                               isBad bool not null default false
                           );
                           """;
    
        var command = new MySqlCommand(sql, conn);
        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}