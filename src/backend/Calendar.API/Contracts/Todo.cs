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

    public Domain.Model.DateTime? Start { get; init; }

    public Domain.Model.DateTime? Due { get; init; }

    public Domain.Model.Duration? Duration { get; init; }

    public Summary? Summary { get; init; }

    public Description? Description { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public TodoStatus Status { get; init; }

    public Location? Location { get; init; }

    public GeographicPosition? GeographicPosition { get; init; }

    public RecurrenceRule? RecurrenceRule { get; init; }

    public Priority? Priority { get; init; }

    public Alarm? Alarm { get; init; }

    public Classification? Classification { get; init; }

    [Description("This field is readonly")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public Instant? Completed { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public Instant Created { get; init; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public Instant? LastModified { get; init; }

    public IReadOnlyCollection<Link> Links { get; init; } = [];
}
