namespace OnCallProber.Configs;

public class OnCallExporterConfiguration
{
    public string? ApiUrl { get; set; }
    public int ScrapeInterval { get; set; }
    public LogLevel LogLevel { get; set; }
    public int MetricsPort { get; set; }
    
    public string? AppKey { get; set; }
    
    public string? AppName { get; set; }
}