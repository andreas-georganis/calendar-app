using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

public enum EventStatus
{
    Confirmed,
    Tentative,
    Cancelled
}

public class Event : Entry
{
    private readonly HashSet<Attendee>? _attendees;
    protected Event() { }

    public Event(
        Guid userId, 
        Guid calendarId, 
        string summary, 
        string? description, 
        DateTime? start, 
        DateTimeOrDuration? end, 
        Alarm? alarm,
        RecurrenceRule? recurrenceRule, 
        string? location, 
        GeographicPosition? geographicPosition,
        IEnumerable<Attendee>? attendees)
        : base(userId, calendarId, summary, description, alarm, recurrenceRule, location, geographicPosition)
    {
        // if (start > end)
        //     throw new ArgumentException("Start date must be before end date");
        
        Start = start;
        End = end;
        
        _attendees = attendees is not null? [..attendees] : null;
        
        Status = EventStatus.Confirmed;
    }
    
    public DateTime? Start { get; init; }
    
    public DateTimeOrDuration? End { get; init; }
    
    public IReadOnlyCollection<Attendee>? Attendees { get; private set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public EventStatus? Status { get; init; }
    
    public bool Edit(string? title, string? description, DateTimeOrDuration? end, string? location, GeographicPosition? geolocation)
    {
        Summary = title;
        return true;
    }
}
