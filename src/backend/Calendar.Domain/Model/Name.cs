using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<Name>))]
public sealed class Name : IParsable<Name>
{
    public string Value { get; init; }

    public Name(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

        Value = value;
    }

    public override string ToString() => Value;

    public static Name Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new FormatException($"Invalid Name: '{s}'.");

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Name result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = null!;
            return false;
        }

        result = new Name(s);
        return true;
    }
}
