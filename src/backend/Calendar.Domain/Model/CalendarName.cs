using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<CalendarName>))]
public sealed class CalendarName : IParsable<CalendarName>
{
    public string Value { get; init; }

    public CalendarName(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

        Value = value;
    }

    public override string ToString() => Value;

    public static CalendarName Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new FormatException($"Invalid Name: '{s}'.");

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out CalendarName result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = null!;
            return false;
        }

        result = new CalendarName(s);
        return true;
    }
}
