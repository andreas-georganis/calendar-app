using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public readonly record struct BySetPos: IParsable<BySetPos>
{
    public IEnumerable<SetPos> Value { get; }

    public BySetPos(IEnumerable<SetPos> value)
    {
        Value = value.ToImmutableHashSet();
    }

    public static BySetPos Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var pos) ? pos : throw new FormatException();

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out BySetPos result)
    {
        var segments = s?.Split(",", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (segments is null)
        {
            result = default;
            return false;
        }

        try
        {
            result = new BySetPos(segments.Select(segment => SetPos.Parse(segment, provider)));
            return true;
        }
        catch (ArgumentOutOfRangeException)
        {
            result = default;
            return false;
        }
    }
}
