
using Calendar.Domain.Exceptions;

using Ical.Net;
using Ical.Net.DataTypes;
using Ical.Net.Evaluation;

using NodaTime;

namespace Calendar.Domain.Model;


public sealed class Todo
{
    private Todo() { }
    
    public Todo(
        UserId userId, 
        CalendarId calendarId,
        TodoId id,
        Summary? summary, 
        Description? description,
        CalDateTime? start,
        CalDateTime? due,
        Duration? duration,
        Priority? priority,
        Alarm? alarm,
        RecurrenceRule? recurrenceRule,
        RecurrencePeriods? recurrencePeriods,
        RecurrenceDates? recurrenceDates,
        ExceptionDates? exceptionDates,
        Location? location,
        GeographicPosition? geographicPosition, 
        Classification? classification,
        Instant created)
    {
        if (due is not null && duration is not null)
        {
            throw new CalendarDomainException("Due date and duration cannot be set at the same time");
        }

        if (due is not null && start is not null && due < start)
        {
            throw new CalendarDomainException("Due date must be after start date");
        }

        if (recurrencePeriods is not null && recurrenceDates is not null)
        {
            throw new CalendarDomainException("Recurrence periods and recurrence dates cannot be set at the same time");
        }

        UserId = userId;
        CalendarId = calendarId;
        Id = id;
        Summary = summary;
        Description = description;
        Start = start;
        Due = due;
        Duration = duration;
        Priority = priority;
        Alarm = alarm;
        RecurrenceRule = recurrenceRule;
        RecurrencePeriods = recurrencePeriods;
        RecurrenceDates = recurrenceDates;
        ExceptionDates = exceptionDates;
        Location = location;
        GeographicPosition = geographicPosition;
        Classification = classification;
        Created = created;

        Status = TodoStatus.NeedsAction;
    }
    
    public UserId UserId { get; }
    public CalendarId CalendarId { get; }
    public TodoId Id { get; }
    public Summary? Summary { get; private set; }

    public Description? Description { get; private set; }
    public CalDateTime? Start { get; }
    public GeographicPosition? GeographicPosition { get; private set; } 

    public RecurrenceRule? RecurrenceRule { get; private set; }
    public RecurrencePeriods? RecurrencePeriods { get; }
    public RecurrenceDates? RecurrenceDates { get; }
    public ExceptionDates? ExceptionDates { get; }

    public Alarm? Alarm { get; private set; }

    public Location? Location { get; private set; }

    public SequenceNumber SequenceNumber { get; private set; } = SequenceNumber.Zero;
    
    public Instant Created { get; private set; }
    
    public Instant? LastModified { get; private set; }
    public CalDateTime? Due { get; }
    public Duration? Duration { get; }
    public Priority? Priority { get; private set; }

    public TodoStatus Status { get; private set; }
    
    
    public Instant? Completed { get; private set; }

    public Classification? Classification { get; init; }

    public bool Edit(Summary? summary, Description? description, Location? location, GeographicPosition? geographicPosition, Alarm? alarm, RecurrenceRule? recurrenceRule, Priority? priority)
    {
        Summary = summary;
        Description = description;
        Location = location;
        GeographicPosition = geographicPosition;
        Alarm = alarm;
        RecurrenceRule = recurrenceRule;
        Priority = priority;

        return true;
    }
    
    public void Complete(IClock clock)
    {
        if (Status is TodoStatus.Completed)
        {
            return;
        }
        
        Status = TodoStatus.Completed;
        Completed = clock.GetCurrentInstant();
    }

    public IEnumerable<Todo> GetOccurrences(CalDateTime? start = null, CalDateTime? end = null)
    {
        var icalTodo = this.ToIcal();

        Ical.Net.DataTypes.CalDateTime? startTime = null;
        Ical.Net.DataTypes.CalDateTime? dueTime = null;

        start ??= this.Start;
        end ??= this.Due;

        if (start is not null)
        {
            startTime = new Ical.Net.DataTypes.CalDateTime(start.Date.Year, start.Date.Month, start.Date.Day, start.Time?.Hour ?? 0, start.Time?.Minute ?? 0, start.Time?.Second ?? 0, start.Zone?.Id);
        }

        var evaluationOptions = new EvaluationOptions
        {
            
        };

        var occurrences = icalTodo.GetOccurrences(startTime, evaluationOptions);

          if (end is not null)
        {
            dueTime = new Ical.Net.DataTypes.CalDateTime(end.Date.Year, end.Date.Month, end.Date.Day, end.Time?.Hour ?? 0, end.Time?.Minute ?? 0, end.Time?.Second ?? 0, end.Zone?.Id);
            occurrences = occurrences.TakeWhileBefore(dueTime);
        }

        return occurrences.Select(o => o.Source.ToDomain()).OfType<Todo>();
    }
}
