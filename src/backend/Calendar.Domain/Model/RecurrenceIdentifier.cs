namespace Calendar.Domain.Model;


public sealed class RecurrenceIdentifier
{
    public required CalDateTime Start { get; init; }
    
    public required RecurrenceIdentifierRange Range { get; init; }

}
