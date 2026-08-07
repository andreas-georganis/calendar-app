using System.Text.Json.Serialization;
using Calendar.Domain.Exceptions;
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
        RecurrencePeriods? recurrenceDates,
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
    public RecurrencePeriods? RecurrenceDates { get; }
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

   
}
