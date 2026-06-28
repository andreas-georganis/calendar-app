using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;

namespace Calendar.Domain.Model;

public readonly record struct ByWeekNo: IParsable<ByWeekNo>
{
    public IEnumerable<YearWeek> Value { get; }

    public ByWeekNo(IEnumerable<YearWeek> value)
    {
        Value = value.ToImmutableHashSet();
    }

    public static ByWeekNo Parse(string s, IFormatProvider? provider)
    {
        if (!TryParse(s, provider, out var result))
        {
            throw new ArgumentException("Could not parse ByWeekNo value.", nameof(s));
        }

        return result;
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ByWeekNo result)
    {
        var segments = s?.Split(',', StringSplitOptions.RemoveEmptyEntries 
                                         | StringSplitOptions.TrimEntries);
        
        if (segments is null or {Length:0})
        {
            result = default;
            return false;
        }

        var weekNos = new List<YearWeek>();
        foreach (var segment in segments)
        {
            if (!YearWeek.TryParse(segment, provider, out var weekNo))
            {
                result = default;
                return false;
            }
            weekNos.Add(weekNo);
        }

        result = new ByWeekNo(weekNos);
        return true;
    }
}
