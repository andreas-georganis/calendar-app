using System.Text.Json.Serialization;
using NodaTime;

namespace Calendar.Domain.Model;

public enum TodoStatus
{
    NeedsAction,
    Completed,
    InProcess,
    Cancelled
}



public sealed class Todo : Entry
{
    private Todo() { }
    
    [JsonConstructor]
    public Todo(
        Guid userId, 
        Guid calendarId, 
        string summary, 
        string? description,
        DateTime? start, 
        DateTimeOrDuration? due,
        Priority? priority,
        Alarm? alarm,
        RecurrenceRule? recurrenceRule,
        string? location,
        GeographicPosition? geographicPosition)
        : base(userId, calendarId, summary, description, alarm, recurrenceRule, location, geographicPosition)
    {
        Start = start;
        Due = due;
        Priority = priority;
        
        Status = TodoStatus.NeedsAction;
    }
    
    
    public DateTime? Start { get; private set; }
    
    public DateTimeOrDuration? Due { get; private set; }
    
    public Priority? Priority { get; private set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public TodoStatus? Status { get; private set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public Instant? Completed { get; private set; }
    
    public bool Edit(string? title, string? description, DateTimeOrDuration? due, Priority? priority)
    {
        return true;
    }
    
    public void Complete(Instant? when = null)
    {
        if (Status is TodoStatus.Completed)
        {
            return;
        }
        
        Status = TodoStatus.Completed;
        Completed = when ?? Instant.FromDateTimeUtc(System.DateTime.UtcNow);
    }
}
