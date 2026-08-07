using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<Description>))]
public sealed class Description : IParsable<Description>
{
    public Description(string? text)
    {
        const int MaxLength = 150;

        Text = text switch
        {
            string {Length: > MaxLength} _ => throw new ArgumentOutOfRangeException($"Description cannot exceed {MaxLength} characters."),
            _ when string.IsNullOrWhiteSpace(text) => "(No title)",
            _ => text
        };
    }

    public string Text { get; }

    public static Description Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result)? result: throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Description result)
    {
        result = new Description(s);
        return true;
    }

    public override string ToString()
        => Text;
}
