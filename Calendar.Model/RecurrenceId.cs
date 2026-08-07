namespace Calendar.Contracts;

public enum RecurrenceIdentifierRange
{
    ThisInstance,
    ThisAndFuture,
}

public sealed class RecurrenceIdentifier
{
    public required DateTime Start { get; init; }
    
    public required RecurrenceIdentifierRange Range { get; init; }
}
