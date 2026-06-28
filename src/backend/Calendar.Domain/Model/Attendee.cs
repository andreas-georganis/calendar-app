namespace Calendar.Domain.Model;

public enum Role
{
    Required,
    Optional,
    Chair,
    NonParticipant
}

public enum ParticipationStatus
{
    NeedsAction,
    Accepted,
    Declined,
    Tentative,
    Delegated,
    Completed,
    InProcess
}


public sealed class Attendee
{
    public required Uri Address { get; init; }
    
    public string? CommonName { get; init; }
    
    public IReadOnlyCollection<Uri>? DelegatedTo { get; init; } = [];

    public IReadOnlyCollection<Uri>? DelegatedFrom { get; init; } = [];
    
    public bool? Rsvp { get; init; }
    
    public Role? Role { get; init; }
    
    public ParticipationStatus? ParticipationStatus { get; init; }
    
    public Uri? SentBy { get; init; }
    
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
}
