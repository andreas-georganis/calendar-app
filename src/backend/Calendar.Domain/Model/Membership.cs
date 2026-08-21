using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;

[JsonConverter(typeof(ParsableJsonConverter<Membership>))]
public sealed class Membership : IEnumerable<CalAddress>, IParsable<Membership>
{
    private readonly HashSet<CalAddress> _members = [];

    internal Membership() { }

    private Membership(HashSet<CalAddress> members)
    {
        _members = members;
    }

    public IEnumerator<CalAddress> GetEnumerator() => _members.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public static Membership Parse(string s, IFormatProvider? provider)
    => TryParse(s, provider, out var result) ? result : throw new FormatException($"Invalid Membership: '{s}'.");

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out Membership result)
    {
        var segments = s?.Split([','], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (segments is null || segments.Length == 0)
        {
            result = null!;
            return false;
        }

        var members = new HashSet<CalAddress>();
        foreach (var segment in segments)
        {
            if (CalAddress.TryParse(segment, provider, out var calAddress))
            {
                members.Add(calAddress);
            }
            else
            {
                result = null!;
                return false;
            }
        }

        result = new Membership(members);
        return true;
    }
}