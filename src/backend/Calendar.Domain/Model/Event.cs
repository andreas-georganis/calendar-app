using System.Reflection;
using System.Text.Json.Serialization;
using Calendar.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using NodaTime;

namespace Calendar.Domain.Model;



// public readonly record struct EventDateRange(DateTime Start, DateTimeOrDuration? End)
// {
//     public DateTimeOrDuration? End {get; init; } = End is { IsDateTime: true } end && end.DateTime < Start ? throw new ArgumentException("End date must be after start date") : End;
// }

public class Event
{
    private readonly HashSet<Attendee>? _attendees;
    protected Event() { }

    public Event(
        UserId userId, 
        CalendarId calendarId, 
        EventId id,
        Summary summary, 
        Description? description,
        DateTime? start,
        DateTime? end,
        Duration? duration,
        Alarm? alarm,
        RecurrenceRule? recurrenceRule, 
        RecurrencePeriods? recurrenceDates,
        ExceptionDates? exceptionDates,
        Location? location, 
        GeographicPosition? geographicPosition,
        IEnumerable<Attendee>? attendees, 
        Instant created)
    {
        
        _attendees = attendees is not null ? [.. attendees] : null;
        
        
        UserId = userId;
        CalendarId = calendarId;
        Id = id;
        Summary = summary;
        Description = description;
        Start = start;

        if (end is not null && duration is not null)
        {
            throw new CalendarDomainException("End date and duration cannot be set at the same time");
        }

        if (end is not null && start is not null && end < start)
        {
            throw new CalendarDomainException("End date must be after start date");
        }

        End = end;
        Duration = duration;

        Alarm = alarm;
        RecurrenceRule = recurrenceRule;
        RecurrenceDates = recurrenceDates;
        ExceptionDates = exceptionDates;
        Location = location;
        GeographicPosition = geographicPosition;
        Created = created;

        Status = EventStatus.Confirmed;
    }

    public Summary? Summary { get; protected set; }

    public Description? Description { get; protected set; }
    public DateTime? Start { get; }
    public GeographicPosition? GeographicPosition { get; private set; } 

    public RecurrenceRule? RecurrenceRule { get; private set; }
    public RecurrencePeriods? RecurrenceDates { get; }
    public ExceptionDates? ExceptionDates { get; }
    public DateTime? End { get; }
    public Alarm? Alarm { get; private set; }
    public Duration? Duration { get; }

    public Location? Location { get; protected set; }

    public SequenceNumber SequenceNumber { get; private set; } = SequenceNumber.Zero;
    
    public Instant Created { get; private set; }
    
    public Instant? LastModified { get; private set; }
    
    public IReadOnlyCollection<Attendee>? Attendees => _attendees?.ToList().AsReadOnly();
    
    public EventStatus Status { get; }

    public Classification? Classification { get;  }

    public TimeTransparency? Transparency { get; }
    public UserId UserId { get; }
    public CalendarId CalendarId { get; }
    public EventId Id { get; }

    public bool Edit(Event @event)
    {
        Summary = @event.Summary;
        return true;
    }
}
