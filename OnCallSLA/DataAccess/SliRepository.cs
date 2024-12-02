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
                           USE sla;
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
    
    public async Task<List<Sli>> Get(CancellationToken cancellationToken, DateTimeOffset start)
    {
        List<Sli> sliList = []; 
        const string sql = """
                           USE sla;
                           SELECT * FROM `sli`
                           WHERE datetime >= @DateTime;
                           """;
        
        var command = new MySqlCommand(sql, _mySqlConnection);
        command.Parameters.AddWithValue("@DateTime", start);
        await _mySqlConnection.OpenAsync(cancellationToken);
        
        var reader  = await command.ExecuteReaderAsync(cancellationToken);
        if (reader.HasRows) 
        {
            while (await reader.ReadAsync(cancellationToken))
            {
                var dateTime = reader.GetDateTimeOffset("datetime");
                var name = reader.GetString("name");
                var slo = reader.GetFloat("slo");
                var value = reader.GetFloat("value");
                var isBad = reader.GetBoolean("isBad");
                
                sliList.Add(new Sli(dateTime, name, slo, value, isBad));
            }
        }
 
        await reader.CloseAsync();
        await _mySqlConnection.CloseAsync();
        return sliList;
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