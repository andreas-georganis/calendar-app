using System.Text.Json.Serialization;
using NodaTime;

namespace Calendar.Domain.Model;

public abstract class Entry
{
    protected Entry() { }

    protected Entry(
        Guid userId, 
        Guid calendarId, 
        string summary, 
        string? description, 
        Alarm? alarm,
        RecurrenceRule? recurrenceRule,
        RecurrenceDateTimes? recurrenceDates,
        ExceptionDateTimes? exceptionDates,
        string? location,
        GeographicPosition? geographicPosition)
    {
        UserId = userId;
        CalendarId = calendarId;
        Summary = summary;
        Description = description;
        Alarm = alarm;
        RecurrenceRule = recurrenceRule;
        Location = location;
        GeographicPosition = geographicPosition;

        Created = Instant.FromDateTimeUtc(System.DateTime.UtcNow);
    }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public Guid Id { get; private set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public Guid UserId { get; private set; }
    
    public Guid CalendarId { get; private set; }
    
    public string? Summary { get; protected set; }

    public string? Description { get; protected set; }

    public GeographicPosition? GeographicPosition { get; private set; } 

    public RecurrenceRule? RecurrenceRule { get; private set; }

    public Alarm? Alarm { get; private set; }

    public string? Location { get; protected set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public Instant Created { get; private set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public Instant? LastModified { get; private set; }
}
