using System.ComponentModel;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using Calendar.Domain.Model;
using NodaTime;

namespace Calendar.API.Contracts;

public sealed class Todo
{
    public CalendarId? CalendarId { get; init; }

    public TodoId Id { get; init; } = TodoId.New();

    public Summary? Summary { get; init; }

    public Description? Description { get; init; }

    public Domain.Model.CalDateTime? Start { get; init; }

    public Domain.Model.CalDateTime? Due { get; init; }

    public Domain.Model.Duration? Duration { get; init; }
    

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public TodoStatus Status { get; init; }

    public Location? Location { get; init; }

    public GeographicPosition? GeographicPosition { get; init; }

    public RecurrenceRule? RecurrenceRule { get; init; }

    public RecurrencePeriods? RecurrencePeriods { get; init; }

    public RecurrenceDates? RecurrenceDates { get; init; }

    public ExceptionDates? ExceptionDates { get; init; }

    public Priority? Priority { get; init; }

    public Alarm? Alarm { get; init; }

    public Classification? Classification { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public Instant? Completed { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public Instant Created { get; init; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public Instant? LastModified { get; init; }

    public IReadOnlyCollection<Link> Links { get; init; } = [];
}
