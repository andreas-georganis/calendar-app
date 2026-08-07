using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using NodaTime;

namespace Calendar.Contracts;

public enum TriggerRelation
{
    Start,
    End
}

[JsonPolymorphic]
[JsonDerivedType(typeof(RelativeTrigger))]
[JsonDerivedType(typeof(AbsoluteTrigger))]
public abstract class Trigger;

public class RelativeTrigger(Duration Duration, TriggerRelation Relation) : Trigger;
    
public class AbsoluteTrigger(Instant Utc): Trigger;

public enum AlarmAction
{
    Audio,
    Display,
    Email
}

public sealed class Repeat
{
    public required int Value { get; init; }
    
    [Required]
    public required Duration Duration { get; init; }
}

[JsonPolymorphic]
[JsonDerivedType(typeof(UriAttachment))]
[JsonDerivedType(typeof(BinaryAttachment))]
public abstract record Attachment
{
    public record UriAttachment([Required]Uri Value) : Attachment;
    
    public record BinaryAttachment([Required]byte[] Value) : Attachment;
}


public sealed class Alarm
{
    public AlarmAction Action { get; init; }
    
    [Required]
    public required Trigger Trigger { get; init; }
    
    public int? RepeatCount { get; init; }
    
    public Duration? RepeatDuration { get; init; }
    
    public IReadOnlyCollection<Attachment> Attach { get; init; }
    
    public string Description { get; init; }
    
    public string Summary { get; init; }
    
    public IReadOnlyCollection<Attendee> Attendees { get; init; } = [];
}

