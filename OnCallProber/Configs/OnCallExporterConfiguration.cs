namespace OnCallProber.Configs;

public class OnCallExporterConfiguration
{
    public string OnCall = "OnCall";
    public string? ApiUrl { get; set; }
    public int ScrapeInterval { get; set; }
    
    public string? AppKey { get; set; }
    
    public string? AppName { get; set; }
}