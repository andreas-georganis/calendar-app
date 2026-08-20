using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<Description>))]
public sealed class Description : IParsable<Description>
{
    public Description(string value)
    {
        const int MaxLength = 150;

        Value = value switch
        {
            string {Length: > MaxLength} _ => throw new ArgumentOutOfRangeException($"Description cannot exceed {MaxLength} characters."),
            _ => value
        };
    }

    public string Value { get; }

    public static Description Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result)? result: throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Description result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = null!;
            return false;
        }

        result = new Description(s);
        return true;
    }

    public override string ToString()
        => Value;
}
