using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<CalAddress>))]
public sealed record CalAddress : IParsable<CalAddress>
{
    public Uri Value { get; init; }

    public CalAddress(Uri value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        Value = value;
    }

    public override string ToString() => Value.ToString();

    public bool IsMailto => Value.Scheme.Equals("mailto", StringComparison.OrdinalIgnoreCase);

    public static CalAddress Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new FormatException($"Invalid CalAddress: '{s}'.");

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out CalAddress result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = null!;
            return false;
        }

        result = new CalAddress(new Uri(s, UriKind.Absolute));
        return true;
    }

    // private static Uri? ToMailto(Uri? uri)
    // {
    //     if (uri is null)
    //         return null;

    //     if (uri.Scheme.Equals("mailto", StringComparison.OrdinalIgnoreCase))
    //         return uri;

    //     // only accept something that actually resembles an email
    //     var candidate = uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString;

    //     var atIndex = candidate.IndexOf('@');
    //     if (atIndex < 1 || atIndex == candidate.Length - 1)
    //         return null; // cannot safely convert

    //     return new Uri($"mailto:{candidate}", UriKind.Absolute);
    // }

    public static implicit operator CalAddress(Uri uri) => new CalAddress(uri);

    public static implicit operator Uri(CalAddress calAddress) => calAddress.Value;
}