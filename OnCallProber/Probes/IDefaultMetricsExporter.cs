namespace OnCallProber.Probes;

public interface IDefaultMetricsExporter
{
    void IncreaseProbeTotal();
    
    void IncreaseProbeSuccess();
    
    void IncreaseProbeFailure();
    
    Prometheus.ITimer NewTimer();
}