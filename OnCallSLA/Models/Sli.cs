namespace OnCallSLA.Models;

public record Sli(DateTimeOffset DateTime,
    string Name,
    float Slo,
    float Value,
    bool IsBad);