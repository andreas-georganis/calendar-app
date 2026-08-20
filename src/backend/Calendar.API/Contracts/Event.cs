using System.Text.Json.Serialization;
using Calendar.Domain.Model;
using NodaTime;
using System.ComponentModel.DataAnnotations;

namespace Calendar.API.Contracts;

public sealed class Event
{
    public CalendarId? CalendarId { get; init; }

    public Domain.Model.EventId Id { get; init; } = Domain.Model.EventId.New();

    [Required]
    public Domain.Model.CalDateTime Start { get; init; }

    public Domain.Model.CalDateTime? End { get; init; }

    public Domain.Model.Duration? Duration { get; init; }

    public Summary? Summary { get; init; }

    public Description? Description { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public EventStatus Status { get; init; }

    public Location? Location { get; init; }

    public GeographicPosition? GeographicPosition { get; init; }

    public RecurrenceRule? RecurrenceRule { get; init; }

    public RecurrencePeriods? RecurrencePeriods { get; init; }

    public RecurrenceDates? RecurrenceDates { get; init; }

    public ExceptionDates? ExceptionDates { get; init; }

    public Alarm? Alarm { get; init; }

    public Classification? Classification { get; init; }

    public TimeTransparency? Transparency { get; init; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public Instant Created { get; init; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenReading)]
    public Instant? LastModified { get; init; }

    public IEnumerable<Attendee>? Attendees { get; init; }

    public IReadOnlyCollection<Link> Links { get; init; } = [];
}