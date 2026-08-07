using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

[JsonConverter(typeof(UntilOrCountJsonConverter))]
public readonly record struct UntilOrCount : IParsable<UntilOrCount>
{
    public static UntilOrCount Forever => new(null, null);

    public static UntilOrCount On(DateTime value)
    {
        return new(value, null);
    }

    public static UntilOrCount After(Count value)
        => new(null, value);
    
    private UntilOrCount(DateTime? until, Count? count)
    {
        Until = until;
        Count = count;
    }

    public DateTime? Until { get; }
    public Count? Count { get; }
    
    public static UntilOrCount Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result: throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out UntilOrCount result)
    {
        DateTime.TryParse(s, provider, out var until);
        if (until != default)
        {
            result = new(until, null);
            return true;
        }

        Domain.Model.Count.TryParse(s, provider, out var count);
        if (count != default)
        {
            result = new(null, count);
            return true;
        }

        result = default;
        return false;
    }
}

public sealed class UntilOrCountJsonConverter : System.Text.Json.Serialization.JsonConverter<UntilOrCount>
{
    public override UntilOrCount Read(ref System.Text.Json.Utf8JsonReader reader, Type typeToConvert, System.Text.Json.JsonSerializerOptions options)
    {
        var doc = JsonDocument.ParseValue(ref reader);

        var root = doc.RootElement;

        bool hasUntil = root.TryGetProperty("until", out var untilProperty) && untilProperty.ValueKind != JsonValueKind.Null;
        bool hasCount = root.TryGetProperty("count", out var countProperty) && countProperty.ValueKind != JsonValueKind.Null;

        if (!(hasUntil ^ hasCount))
        {
            throw new JsonException("Either 'until' or 'count' must be provided");
        }
        
        if (hasUntil && DateTime.TryParse(untilProperty.GetString(), null, out var until))
        {
            return UntilOrCount.On(until);
        }

        if (hasCount && countProperty.TryGetInt32(out var count))
        {
            return UntilOrCount.After(new Count(count));
        }

        return UntilOrCount.Forever;
    }

    public override void Write(System.Text.Json.Utf8JsonWriter writer, UntilOrCount value, System.Text.Json.JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (value.Until is not null)
        {
             writer.WriteString("until", value.Until.ToString());
        }

        if (value.Count is not null)
        {
            writer.WriteNumber("count", value.Count.Value.Value);
        }

        writer.WriteEndObject();
        
    }
}
