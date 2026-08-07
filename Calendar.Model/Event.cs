using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NodaTime;

namespace Calendar.Contracts;

public enum EventStatus
{
    Confirmed,
    Tentative,
    Cancelled
}

public class Event : Entry
{
    public DateTime? Start { get; init; }
    
    public DateTime? End { get; init; }
    
    public Duration? Duration { get; init; }
    
    public string? Description { get; init; }
    
    public Recurrence? Recurrence { get; init; }
    
    public string? Location { get; init; }
    
    public GeographicPosition? GeographicPosition { get; init; }
    
    public Alarm? Alarm { get; init; }
    
    public IReadOnlyCollection<Attendee>? Attendees { get; init; }
    
    [ViewOnly]
    public EventStatus? Status { get; init; }
}
