using System.Text.Json.Serialization;
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
        Summary summary, 
        Description? description,
        DateTime? start,
        DateTime? due,
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
        UserId = userId;
        CalendarId = calendarId;
        Id = id;
        Summary = summary;
        Description = description;
        Start = start;

        if (due is not null && duration is not null)
        {
            throw new CalendarDomainException("Due date and duration cannot be set at the same time");
        }

        if (due is not null && start is not null && due < start)
        {
            throw new CalendarDomainException("Due date must be after start date");
        }

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
    public DateTime? Start { get; }
    public GeographicPosition? GeographicPosition { get; private set; } 

    public RecurrenceRule? RecurrenceRule { get; private set; }
    public RecurrencePeriods? RecurrencePeriods { get; }
    public RecurrenceDates? RecurrenceDates { get; }
    public ExceptionDates? ExceptionDates { get; }

    public Alarm? Alarm { get; private set; }

    public Location? Location { get; private set; }

    [JsonIgnore]
    public SequenceNumber SequenceNumber { get; private set; } = SequenceNumber.Zero;
    
    public Instant Created { get; private set; }
    
    public Instant? LastModified { get; private set; }
    public DateTime? Due { get; }
    public Duration? Duration { get; }
    public Priority? Priority { get; private set; }

    public TodoStatus Status { get; private set; }
    
    
    public Instant? Completed { get; private set; }

    public Classification? Classification { get; init; }

    public bool Edit(Todo todo)
    {
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

    public IEnumerable<Todo> GetOccurrences(DateTime? start = null, DateTime? end = null)
    {
        var icalTodo = this.ToIcal();

        CalDateTime? startTime = null;
        CalDateTime? dueTime = null;

        start ??= this.Start;
        end ??= this.Due;

        if (start is not null)
        {
            startTime = new CalDateTime(start.Value.Date.Year, start.Value.Date.Month, start.Value.Date.Day, start.Value.Time?.Hour ?? 0, start.Value.Time?.Minute ?? 0, start.Value.Time?.Second ?? 0, start.Value.Zone?.Id);
        }

        var evaluationOptions = new EvaluationOptions
        {
            
        };

        var occurrences = icalTodo.GetOccurrences(startTime, evaluationOptions);

          if (end is not null)
        {
            dueTime = new CalDateTime(end.Value.Date.Year, end.Value.Date.Month, end.Value.Date.Day, end.Value.Time?.Hour ?? 0, end.Value.Time?.Minute ?? 0, end.Value.Time?.Second ?? 0, end.Value.Zone?.Id);
            occurrences = occurrences.TakeWhileBefore(dueTime);
        }

        return occurrences.Select(o => o.Source.ToDomain()).OfType<Todo>();
    }
}
