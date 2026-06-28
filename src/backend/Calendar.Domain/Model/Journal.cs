namespace Calendar.Domain.Model;

public enum JournalStatus
{
    Draft,
    Final,
    Cancelled
}

public class Journal: Entry
{
    public DateTime? Start { get; init; }
    
    public JournalStatus? Status { get; init; }
    
    public IReadOnlySet<Attendee>? Attendees { get; init; }
}
