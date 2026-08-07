using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using NodaTime;

namespace Calendar.Domain.Model;

public sealed class File
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public Guid Id { get; init; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public Guid UserId { get; init; }
    
    public required string OriginalName { get; init; }
    public required string ContentType { get; init; }
    public required string ContentDisposition { get; init; }
    public required long Size { get; init; }

    [JsonIgnore]
    public SaveResult? SaveResult { get; set; } 
    
    [JsonIgnore]

    public ScanResult? ScanResult { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public Instant Created { get; init; }
}

public enum ScanStatus
{
    Pending,
    InProgress,
    Clean,
    Infected,
    Failed
}

public sealed record ScanResult(
    ScanStatus Status,
    string? ThreatName,
    string? Engine,
    Instant Created);

public sealed record SaveResult(
    bool Success,
    Uri? Uri);
