namespace Calendar.Domain.Model;


public sealed class Journal
{
    public CalDateTime? Start { get; init; }
    
    public JournalStatus? Status { get; init; }
    
    public IReadOnlySet<Attendee>? Attendees { get; init; }
}
