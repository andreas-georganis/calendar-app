using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace Calendar.Contracts;

public enum JournalStatus
{
    Draft,
    Final,
    Cancelled
}

public class Journal: Entry
{
    public DateTime? Start { get; init; }
    
    public string? Description { get; init; }
    
    public JournalStatus? Status { get; init; }
    
    public Recurrence? Recurrence { get; init; }
    
    public IReadOnlySet<Attendee>? Attendees { get; init; }
}
