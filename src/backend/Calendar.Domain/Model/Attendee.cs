namespace Calendar.Domain.Model;

public sealed class Attendee : IEquatable<Attendee>
{
    public required Uri Address { 
        get; 
        init
        {
            var mailto = ToMailto(value);

            field = mailto?? throw new ArgumentException("Address must be a valid email address or a mailto URI.", nameof(value));
        }
    }
    
    public CommonName? CommonName { get; init; }

    public CalendarUserType? CuType { get; init; }

    public IReadOnlyCollection<Uri>? Members { get; init; } = [];
    
    public IReadOnlyCollection<Uri>? DelegatedTo { get; init; } = [];

    public IReadOnlyCollection<Uri>? DelegatedFrom { get; init; } = [];
    
    public bool? Rsvp { get; init; }
    
    public Role? Role { get; init; }
    
    public ParticipationStatus? ParticipationStatus { get; init; }
    
    public Uri? SentBy { 
        get; 
        init
        {
            var mailto = ToMailto(value);

            field = mailto?? throw new ArgumentException("SentBy must be a valid email address or a mailto URI.", nameof(value));
        }
    }
    
    private static Uri? ToMailto(Uri? uri)
    {
        if (uri is null)
            return null;

        if (uri.Scheme.Equals("mailto", StringComparison.OrdinalIgnoreCase))
            return uri;

        // only accept something that actually resembles an email
        var candidate = uri.IsAbsoluteUri ? uri.AbsoluteUri : uri.OriginalString;

        var atIndex = candidate.IndexOf('@');
        if (atIndex < 1 || atIndex == candidate.Length - 1)
            return null; // cannot safely convert

        return new Uri($"mailto:{candidate}", UriKind.Absolute);
    }

    public bool Equals(Attendee? other)
    {
        if (other is null)
            return false;

        return Address.Equals(other.Address) &&
               CommonName == other.CommonName &&
               CuType == other.CuType &&
               Members?.SequenceEqual(other.Members ?? []) == true &&
               DelegatedTo?.SequenceEqual(other.DelegatedTo ?? []) == true &&
               DelegatedFrom?.SequenceEqual(other.DelegatedFrom ?? []) == true &&
               Rsvp == other.Rsvp &&
               Role == other.Role &&
               ParticipationStatus == other.ParticipationStatus &&
               SentBy == other.SentBy;
    }

    public override bool Equals(object? obj) => Equals(obj as Attendee);

    public override int GetHashCode()
    {
        var hashCode = new HashCode();
        hashCode.Add(Address);
        hashCode.Add(CommonName);
        hashCode.Add(CuType);
        if (Members is not null)
            foreach (var member in Members)
                hashCode.Add(member);
        if (DelegatedTo is not null)
            foreach (var delegatedTo in DelegatedTo)
                hashCode.Add(delegatedTo);
        if (DelegatedFrom is not null)
            foreach (var delegatedFrom in DelegatedFrom)
                hashCode.Add(delegatedFrom);
        hashCode.Add(Rsvp);
        hashCode.Add(Role);
        hashCode.Add(ParticipationStatus);
        hashCode.Add(SentBy);
        return hashCode.ToHashCode();
    }
}
