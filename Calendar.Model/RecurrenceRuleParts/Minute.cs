namespace Calendar.API.Model.RecurrenceRuleParts;

public record Minute
{
    public int Value { get; }

    public Minute(int value)
    {
        if (value is < 0 or > 59)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
        
        Value = value;
    }
}
