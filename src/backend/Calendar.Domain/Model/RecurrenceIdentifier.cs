namespace Calendar.Domain.Model;


public sealed class RecurrenceIdentifier
{
    public required DateTime Start { get; init; }
    
    public required RecurrenceIdentifierRange Range { get; init; }

}
