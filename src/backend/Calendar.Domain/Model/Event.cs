using Calendar.Domain.Exceptions;

using Ical.Net;
using Ical.Net.Evaluation;

using NodaTime;

namespace Calendar.Domain.Model;

public class Event
{
    private readonly HashSet<Attendee>? _attendees;

    private Event()
    {
        Start = default!;
        End = default!;
    }

    public Event(
        UserId userId, 
        CalendarId calendarId, 
        EventId id,
        Summary summary, 
        Description? description,
        CalDateTime start,
        CalDateTime? end,
        Duration? duration,
        Alarm? alarm,
        RecurrenceRule? recurrenceRule, 
        RecurrencePeriods? recurrencePeriods,
        RecurrenceDates? recurrenceDates,
        ExceptionDates? exceptionDates,
        Location? location, 
        GeographicPosition? geographicPosition,
        IList<Attendee>? attendees, 
        Instant created)
    {
        if (end is not null && duration is not null)
        {
            throw new CalendarDomainException("End date and duration cannot be set at the same time");
        }

        if (end is not null /*&& start is not null*/ && end < start)
        {
            throw new CalendarDomainException("End date must be after start date");
        }

        if (recurrencePeriods is not null && recurrenceDates is not null)
        {
            throw new CalendarDomainException("Recurrence periods and recurrence dates cannot be set at the same time");
        }
        
        _attendees = attendees is not null ? [.. attendees] : null;
        
        UserId = userId;
        CalendarId = calendarId;
        Id = id;
        Summary = summary;
        Description = description;
        Start = start;
        End = end switch
        {
            not null => end,
            null when Start.IsDateOnly => Start + Domain.Model.Duration.OneDay,
            null => Start,
        };
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

    public UserId UserId { get; }
    public CalendarId CalendarId { get; }
    public EventId Id { get; }

    public Summary? Summary { get; private set; }

    public Description? Description { get; private set; }
    public CalDateTime Start { get; }
    public GeographicPosition? GeographicPosition { get; private set; } 

    public RecurrenceRule? RecurrenceRule { get; private set; }
    public RecurrencePeriods? RecurrencePeriods { get; }
    public RecurrenceDates? RecurrenceDates { get; }
    public ExceptionDates? ExceptionDates { get; }
    public CalDateTime End { get; }
    public Alarm? Alarm { get; private set; }
    public Duration? Duration { get; }

    public Location? Location { get; private set; }

    public SequenceNumber SequenceNumber { get; private set; } = SequenceNumber.Zero;
    
    public Instant Created { get; private set; }
    
    public Instant? LastModified { get; private set; }
    
    public IList<Attendee>? Attendees => _attendees?.ToList().AsReadOnly();
    
    public EventStatus Status { get; }

    public Classification? Classification { get; }

    public TimeTransparency? Transparency { get; }
    

    public bool Edit(Summary? summary, Description? description, Location? location, GeographicPosition? geographicPosition, Alarm? alarm, RecurrenceRule? recurrenceRule)
    {
        Summary = summary;
        Description = description;
        Location = location;
        GeographicPosition = geographicPosition;
        Alarm = alarm;
        RecurrenceRule = recurrenceRule;

        return true;
    }

    public IEnumerable<Event> GetOccurrences(CalDateTime? start = null, CalDateTime? end = null)
    {
        var icalEvent = this.ToIcal();

        Ical.Net.DataTypes.CalDateTime? startTime = null;
        Ical.Net.DataTypes.CalDateTime? dueTime = null;

        start ??= this.Start;
        end ??= this.End;

        if (start is {} startValue)
        {
            startTime = new Ical.Net.DataTypes.CalDateTime(startValue.Date.Year, startValue.Date.Month, startValue.Date.Day, startValue.Time?.Hour ?? 0, startValue.Time?.Minute ?? 0, startValue.Time?.Second ?? 0, startValue.Zone?.Id);
        }

        var evaluationOptions = new EvaluationOptions
        {
            
        };

        var occurrences = icalEvent.GetOccurrences(startTime, evaluationOptions);

        if (end is {} endValue)
        {
            dueTime = new Ical.Net.DataTypes.CalDateTime(endValue.Date.Year, endValue.Date.Month, endValue.Date.Day, endValue.Time?.Hour ?? 0, endValue.Time?.Minute ?? 0, endValue.Time?.Second ?? 0, endValue.Zone?.Id);
            occurrences = occurrences.TakeWhileBefore(dueTime);
        }

        return occurrences.Select(o => o.Source.ToDomain()).OfType<Event>();
    }
}
