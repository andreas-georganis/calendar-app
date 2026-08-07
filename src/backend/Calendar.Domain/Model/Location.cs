namespace Calendar.Domain.Model;

public sealed class Location : IParsable<Location>
{
    public Location(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Location Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new FormatException();

    public static bool TryParse(string? s, IFormatProvider? provider, out Location result)
    {
        if (string.IsNullOrWhiteSpace(s))
        {
            result = default!;
            return true;
        }

        result = new Location(s);
        return true;
    }
}
