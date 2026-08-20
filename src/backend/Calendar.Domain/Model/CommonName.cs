using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<CommonName>))]
public sealed class CommonName : IParsable<CommonName>
{
    public CommonName(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));
        Value = value;
    }

    public string Value { get; }

    public static CommonName Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result)? result: throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out CommonName result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = null!;
            return false;
        }

        result = new CommonName(s);
        return true;
    }

    public override string ToString()
        => Value;
}