using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

using NodaTime;
using NodaTime.Text;

namespace Calendar.Domain.Model;

file interface ITrigger;

[JsonConverter(typeof(TriggerJsonConverter))]
public partial record class Trigger : ITrigger, IValidatableObject//, IParsable<Trigger>
{
    // private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.IgnoreCase;
    
    // [GeneratedRegex(@"^(?:TRIGGER(?:;VALUE=(?<valueType>DURATION)(?:;RELATED=(?<related>START|END))?|;RELATED=(?<related>START|END)(?:;VALUE=(?<valueType>DURATION))?|;VALUE=(?<valueType>DATE-TIME))?:)?(?<value>.+)$", Options, 2000)]
    // private static partial Regex TriggerRegex { get; }
    
    public static Trigger RelativeTrigger(Duration duration, TriggerRelation relation = TriggerRelation.Start) 
        => new Trigger
        {
            Duration = duration,
            Relation = relation
        };
    
    public static Trigger AbsoluteTrigger(Instant utc)
        => new Trigger
        {
            Utc = utc
        };


    /// <summary>
    /// For an absolute trigger, the UTC time at which the alarm will trigger. For a relative trigger, this field is null.
    /// </summary>
    public Instant? Utc { get; init; }
    public Domain.Model.Duration? Duration { get; init; }

    public TriggerRelation? Relation { get; init; }
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Utc is null && Duration is null)
        {
            yield return new ValidationResult(
                "Either Utc or Duration must be specified.",
                new[] { nameof(Utc), nameof(Duration) });
        }
    }
    
    // public static Trigger Parse(string s, IFormatProvider? provider) 
    //     => TryParse(s, provider, out var result) ? result : throw new FormatException();

    // public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Trigger result)
    // {
    //     if (string.IsNullOrWhiteSpace(s))
    //     {
    //         result = default;
    //         return false;
    //     }

    //     var match = TriggerRegex.Match(s);
    //     if (!match.Success)
    //     {
    //         result = default;
    //         return false;
    //     }

    //     var valueType = match.Groups["valueType"].Value;
    //     var value = match.Groups["value"].Value;

    //     if (valueType.Equals("DATE-TIME", StringComparison.InvariantCultureIgnoreCase))
    //     {
    //         var instantResult = InstantPattern.ExtendedIso.Parse(value);
            
    //         if (instantResult.Success)
    //         {
    //             result = AbsoluteTrigger(instantResult.Value);
    //             return true;
    //         }
    //     }
        
    //     if (Model.Duration.TryParse(value, provider, out var duration))
    //     {
    //         var relationValue = match.Groups["relation"].Value;
            
    //         Enum.TryParse(relationValue, true, out TriggerRelation relation);
            
    //         result = RelativeTrigger(duration, relation);
    //         return true;
    //     }

    //     result = default;
    //     return false;
    // }
}

internal sealed class TriggerJsonConverter : JsonConverter<Trigger>
{
    public override Trigger? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
        {
            throw new JsonException("Expected an object for Trigger.");
        }

        Instant? utc = null;
        Duration? duration = null;
        TriggerRelation? relation = null;

        while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
        {
            var propertyName = reader.GetString();
            reader.Read();

            if (reader.TokenType == JsonTokenType.Null)
            {
                continue;
            }

            switch (propertyName)
            {
                case nameof(Trigger.Utc):
                    utc = InstantPattern.ExtendedIso.Parse(reader.GetString()!).Value;
                    break;
                case nameof(Trigger.Duration):
                    duration = Duration.Parse(reader.GetString()!, null);
                    break;
                case nameof(Trigger.Relation):
                    relation = Enum.Parse<TriggerRelation>(reader.GetString()!, true);
                    break;
                default:
                    reader.Skip();
                    break;
            }
        }

        if (utc is { } utcValue)
        {
            return Trigger.AbsoluteTrigger(utcValue);
        }

        if (duration is { } durationValue)
        {
            return Trigger.RelativeTrigger(durationValue, relation ?? TriggerRelation.Start);
        }

        return default;
    }

    public override void Write(Utf8JsonWriter writer, Trigger value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (value.Utc is { } utc)
        {
            writer.WriteString(nameof(Trigger.Utc), InstantPattern.ExtendedIso.Format(utc));
        }

        if (value.Duration is { } duration)
        {
            writer.WriteString(nameof(Trigger.Duration), duration.ToString());
        }

        if (value.Relation is { } relation)
        {
            writer.WriteString(nameof(Trigger.Relation), relation.ToString());
        }

        writer.WriteEndObject();
    }
}