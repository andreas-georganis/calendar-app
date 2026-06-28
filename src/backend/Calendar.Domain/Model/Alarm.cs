namespace Calendar.Domain.Model;

public enum TriggerRelation
{
    Start,
    End
}

public enum AlarmAction
{
    Audio,
    Display,
    Email
}

public readonly record struct Repeat
{
    public int Value { get; }
    public NodaTime.Duration Duration { get; }

    public Repeat(int value, NodaTime.Duration duration)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value, 0, nameof(value));
        Value = value;
        Duration = duration;
    }
}

file interface IAlarm;

public record Alarm : IAlarm
{
    public static Alarm AudioAlarm(
        Trigger trigger,
        string description,
        string summary,
        IEnumerable<Attendee> attendees,
        IEnumerable<Attachment> attachments,
        Repeat? repeat = null) => new Alarm(AlarmAction.Audio, null, null,new EmailAlarm(trigger, description, summary, attendees, attachments, repeat));
    
    private readonly AudioAlarm? _audio;
    private readonly DisplayAlarm? _display;
    private readonly EmailAlarm? _email;

    public AlarmAction Action { get; }
    
    // public Trigger Trigger { get; }
    //
    // public Repeat? Repeat { get; }
    
    private Alarm(AlarmAction action, AudioAlarm? audio, DisplayAlarm? display, EmailAlarm? email)
    {
        Action = action;
        _audio = audio;
        _display = display;
        _email = email;
    }
}

public record EmailAlarm(
    Trigger Trigger,
    string Description,
    string Summary,
    IEnumerable<Attendee> Attendees,
    IEnumerable<Attachment> Attachments,
    Repeat? Repeat = null) : IAlarm;
    
public record DisplayAlarm(Trigger Trigger, string Description, Repeat? Repeat = null): IAlarm;
public record AudioAlarm(Trigger Trigger, Attachment Attach, Repeat? Repeat = null): IAlarm;

