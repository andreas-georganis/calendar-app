namespace Calendar.Domain.Model;

public enum RecurrenceIdentifierRange
{
    ThisInstance,
    ThisAndFuture,
}

public sealed class RecurrenceIdentifier
{
    public required System.DateTime Start { get; init; }
    
    public required RecurrenceIdentifierRange Range { get; init; }
}
