using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<EventId>))]
public readonly record struct EventId : IParsable<EventId>
{
    public static EventId New()
        => new(Guid.CreateVersion7());

    public EventId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static EventId Parse(string s, IFormatProvider? provider)
        => new EventId(Guid.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out EventId result)
    {
        if (Guid.TryParse(s, provider, out var guid))
        {
            result = new EventId(guid);
            return true;
        }
        result = default;
        return false;
    }
}
