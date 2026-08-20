namespace Calendar.Domain.Model;

file interface IAlarm;

public sealed class Alarm : IAlarm
{
    public required AlarmAction Action { get; init; }

    public Summary? Summary { get; init; }

    public Description? Description { get; init; }

    public Repeat? Repeat { get; init; }

    public Duration? Duration { get; init; }

    public required Trigger Trigger { get; init; }

    public IList<Attendee>? Attendees { get; init; } = [];

    public IList<Attachment>? Attachments { get; init; } = [];
}

