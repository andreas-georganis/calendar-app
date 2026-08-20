using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<Summary>))]
public sealed class Summary : IParsable<Summary>
{
    public Summary(string? value)
    {
        const int MaxLength = 30;

        Value = value switch
        {
            string {Length: > MaxLength} _ => throw new ArgumentOutOfRangeException($"Summary should be at most {MaxLength} characters long."),
            _ when string.IsNullOrWhiteSpace(value) => "(No title)",
            _ => value
        };
    }

    public string Value { get; }

    public static Summary Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result)? result: throw new FormatException(); 

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Summary result)
    {
        result = new Summary(s);
        return true;
    }
}
