using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<CalendarId>))]
public readonly record struct CalendarId : IParsable<CalendarId>
{
    public static CalendarId New()
        => new(Guid.CreateVersion7());

    public Guid Value { get; }

    public CalendarId(Guid value)
    {
        Value = value;
    }

    public static CalendarId Parse(string s, IFormatProvider? provider)
        => new(Guid.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out CalendarId result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = default;
            return false;
        }

        if (Guid.TryParse(s, provider, out var guid))
        {
            result = new CalendarId(guid);
            return true;
        }

        result = default;
        return false;
    }
}
