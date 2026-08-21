namespace Calendar.Domain.Model;

public sealed class Attendee : IEquatable<Attendee>
{
    public required CalAddress Value {
        get; 
        init;
    }
    
    public CommonName? CommonName { get; init; }

    public CalendarUserType? CuType { get; init; }

    public Membership? Members { get; init; } = [];
    
    public Delegators? DelegatedTo { get; init; } = [];

    public Delegatees? DelegatedFrom { get; init; } = [];
    
    public bool? Rsvp { get; init; }
    
    public Role? Role { get; init; }
    
    public ParticipationStatus? ParticipationStatus { get; init; }
    
    public CalAddress? SentBy { 
        get; 
        init
        {
            if (value?.IsMailto is false)
                throw new ArgumentException("SentBy must be a valid email address or a mailto URI.", nameof(value));

            field = value;
        }
    }

    public bool Equals(Attendee? other)
    {
        if (other is null)
            return false;

        return Value.Equals(other.Value) &&
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
        hashCode.Add(Value);
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
