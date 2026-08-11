using Calendar.Domain.Exceptions;

using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Evaluation;

using NodaTime;

namespace Calendar.Domain.Model;

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
        RecurrencePeriods? recurrencePeriods,
        RecurrenceDates? recurrenceDates,
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
        RecurrencePeriods = recurrencePeriods;
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
    public RecurrencePeriods? RecurrencePeriods { get; }
    public RecurrenceDates? RecurrenceDates { get; }
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

    public IEnumerable<Event> GetOccurrences(DateTime? start = null, DateTime? end = null)
    {
        var icalEvent = this.ToIcal();

        CalDateTime? startTime = null;
        CalDateTime? dueTime = null;

        start ??= this.Start;
        end ??= this.End;

        if (start is not null)
        {
            startTime = new CalDateTime(start.Value.Date.Year, start.Value.Date.Month, start.Value.Date.Day, start.Value.Time?.Hour ?? 0, start.Value.Time?.Minute ?? 0, start.Value.Time?.Second ?? 0, start.Value.Zone?.Id);
        }

        var evaluationOptions = new EvaluationOptions
        {
            
        };

        var occurrences = icalEvent.GetOccurrences(startTime, evaluationOptions);

          if (end is not null)
        {
            dueTime = new CalDateTime(end.Value.Date.Year, end.Value.Date.Month, end.Value.Date.Day, end.Value.Time?.Hour ?? 0, end.Value.Time?.Minute ?? 0, end.Value.Time?.Second ?? 0, end.Value.Zone?.Id);
            occurrences = occurrences.TakeWhileBefore(dueTime);
        }

        return occurrences.Select(o => o.Source.ToDomain()).OfType<Event>();
    }
}
