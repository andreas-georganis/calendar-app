
using Calendar.Contracts;
using NodaTime;

namespace Calendar.API.Model;

file interface ITrigger;

public sealed record Trigger : ITrigger
{
    public static Trigger RelativeTrigger(Duration duration, TriggerRelation relation = TriggerRelation.Start) 
        => new(new RelativeTrigger(duration, relation), null);
    
    public static Trigger AbsoluteTrigger(Instant utc) 
        => new(null, new AbsoluteTrigger(utc));

    private Trigger(RelativeTrigger? relative, AbsoluteTrigger? absolute)
    {
        Relative = relative;
        Absolute = absolute;
    }

    
    public RelativeTrigger? Relative { get; }

    public AbsoluteTrigger? Absolute { get; }
}

public sealed record RelativeTrigger(Duration Duration, TriggerRelation Relation) : ITrigger;
    
public sealed record AbsoluteTrigger(Instant Utc): ITrigger;
