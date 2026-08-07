namespace Calendar.Domain.Model;

file interface IAlarm;

public sealed class Alarm : IAlarm
{
    public AlarmAction Action { get; init; }

    public Summary? Summary { get; init; }

    public Description? Description { get; init; }

    public Repeat Repeat { get; init; }

    public required Trigger Trigger { get; init; }

    public IEnumerable<Attendee>? Attendees { get; init; } = [];

    public IEnumerable<Attachment>? Attachments { get; init; } = [];
}

