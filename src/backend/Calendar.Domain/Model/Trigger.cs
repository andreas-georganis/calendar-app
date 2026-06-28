using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using NodaTime;

namespace Calendar.Domain.Model;

file interface ITrigger;

public readonly partial record struct Trigger : ITrigger, IParsable<Trigger>
{
    private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.IgnoreCase;
    
    [GeneratedRegex(@"^(?:TRIGGER(?:;VALUE=(?<valueType>DURATION|DATE-TIME))?(?:;RELATED=(?<related>START|END))?:)?(?<value>.+)$", Options, 2000)]
    private static partial Regex TriggerRegex { get; }
    
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
    
    public static Trigger Parse(string s, IFormatProvider? provider) 
        => TryParse(s, provider, out var result) ? result : throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider,
        [MaybeNullWhen(false)] out Trigger result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = default;
            return false;
        }

        var match = TriggerRegex.Match(s);
        if (match.Success)
        {
            result = default;
            return false;
        }

        result = default;
        return false;
    }
}

public readonly record struct RelativeTrigger(Duration Duration, TriggerRelation Relation) : ITrigger;
    
public readonly record struct AbsoluteTrigger(Instant Utc): ITrigger;

// [JsonPolymorphic]
// [JsonDerivedType(typeof(RelativeTrigger))]
// [JsonDerivedType(typeof(AbsoluteTrigger))]
// public abstract class Trigger;
//
// public class RelativeTrigger(Duration Duration, TriggerRelation Relation) : Trigger;
//     
// public class AbsoluteTrigger(Instant Utc): Trigger;
