namespace OnCallSLA.Models;

public record CreateUserSli(DateTimeOffset DateTime, string Name, float Slo, float Value, bool IsBad)
    : Sli(DateTime, Name, Slo, Value, IsBad);