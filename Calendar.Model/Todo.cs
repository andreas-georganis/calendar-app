
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NodaTime;

namespace Calendar.Contracts;

public enum TodoStatus
{
    NeedsAction,
    Completed,
    InProcess,
    Cancelled
}

public enum Priority
{
    Low,
    Medium,
    High
}

public class Todo : Entry
{
    public DateTime? Start { get; init; }
    
    public DateTime? Due { get; init; }
    
    public Duration? Duration { get; init; }
    
    public string? Description { get; init; }
    
    public Recurrence? Recurrence { get; init; }
    
    public string? Location { get; init; }
    
    public GeographicPosition? GeographicPosition { get; init; }
    
    public Alarm? Alarm { get; init; }
    
    public Priority? Priority { get; init; }
    
    [ViewOnly]
    public TodoStatus? Status { get; init; }
    
    [ViewOnly]
    public Instant? Completed { get; init; }
}
