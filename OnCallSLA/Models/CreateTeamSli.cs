namespace OnCallSLA.Models;

public record CreateTeamSli(DateTimeOffset DateTime, string Name, float Slo, float Value, bool IsBad)
    : Sli(DateTime, Name, Slo, Value, IsBad);