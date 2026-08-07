namespace Calendar.Domain.Model;


public sealed class Journal
{
    public DateTime? Start { get; init; }
    
    public JournalStatus? Status { get; init; }
    
    public IReadOnlySet<Attendee>? Attendees { get; init; }
}
