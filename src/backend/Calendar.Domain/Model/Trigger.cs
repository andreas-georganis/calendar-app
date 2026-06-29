using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using NodaTime;
using NodaTime.Text;

namespace Calendar.Domain.Model;

public enum TriggerRelation
{
    
    Start,
    End
}

file interface ITrigger;

public readonly partial record struct Trigger : ITrigger, IParsable<Trigger>
{
    private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.IgnoreCase;
    
    [GeneratedRegex(@"^(?:TRIGGER(?:;VALUE=(?<valueType>DURATION)(?:;RELATED=(?<related>START|END))?|;RELATED=(?<related>START|END)(?:;VALUE=(?<valueType>DURATION))?|;VALUE=(?<valueType>DATE-TIME))?:)?(?<value>.+)$", Options, 2000)]
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

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Trigger result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = default;
            return false;
        }

        var match = TriggerRegex.Match(s);
        if (!match.Success)
        {
            result = default;
            return false;
        }

        var valueType = match.Groups["valueType"].Value;
        var value = match.Groups["value"].Value;

        if (valueType.Equals("DATE-TIME", StringComparison.InvariantCultureIgnoreCase))
        {
            var instantResult = InstantPattern.ExtendedIso.Parse(value);
            
            if (instantResult.Success)
            {
                result = AbsoluteTrigger(instantResult.Value);
                return true;
            }
        }
        
        if (Model.Duration.TryParse(value, provider, out var duration))
        {
            var relationValue = match.Groups["relation"].Value;
            
            Enum.TryParse(relationValue, true, out TriggerRelation relation);
            
            result = RelativeTrigger(duration, relation);
            return true;
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
