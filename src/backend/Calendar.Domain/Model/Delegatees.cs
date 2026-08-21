using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<Delegatees>))]
public sealed class Delegatees : IEnumerable<CalAddress>, IParsable<Delegatees>
{
    private readonly HashSet<CalAddress> _calAddresses;

    internal Delegatees()
    {
        _calAddresses = new HashSet<CalAddress>();
    }

    private Delegatees(HashSet<CalAddress> calAddresses)
    {
        _calAddresses = calAddresses;
    }

    public static Delegatees Parse(string s, IFormatProvider? provider)
        => TryParse(s, provider, out var result) ? result : throw new FormatException($"Invalid Delegatees: '{s}'.");

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Delegatees result)
    {
        var segments = s?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (segments is null || segments.Length == 0)
        {
            result = null!;
            return false; 
        }

        var calAddresses = new HashSet<CalAddress>();
        foreach (var segment in segments)
        {
            if (CalAddress.TryParse(segment, provider, out var calAddress))
            {
                calAddresses.Add(calAddress);
            }
            else
            {
                result = null!;
                return false; 
            }
        }

        result = new Delegatees(calAddresses);
        return true;
    }

    public IEnumerator<CalAddress> GetEnumerator()
    {
        return _calAddresses.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}