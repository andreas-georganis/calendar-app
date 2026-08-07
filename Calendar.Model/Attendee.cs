using System.Text.Json.Serialization;
using Ardalis.SmartEnum;

namespace Calendar.Contracts;

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


public class Attendee
{
    public required Uri Address { get; init; }
    
    public required string Name { get; init; }
    
    public IReadOnlyCollection<Uri>? DelegatedTo { get; init; } = [];

    public IReadOnlyCollection<Uri>? DelegatedFrom { get; init; } = [];
    
    public bool? Rsvp { get; init; }
    
    public Role? Role { get; init; }
    
    public ParticipationStatus? ParticipationStatus { get; init; }
    
    public Uri? SentBy { get; init; }
}
