using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<TodoId>))]
public readonly record struct TodoId : IParsable<TodoId>
{
    public static TodoId New()
        => new(Guid.CreateVersion7());

    public TodoId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static TodoId Parse(string s, IFormatProvider? provider)
        => new TodoId(Guid.Parse(s, provider));

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out TodoId result)
    {
        if (Guid.TryParse(s, provider, out var guid))
        {
            result = new TodoId(guid);
            return true;
        }
        result = default;
        return false;
    }
}
