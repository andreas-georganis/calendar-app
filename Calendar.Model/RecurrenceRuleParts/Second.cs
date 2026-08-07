namespace Calendar.API.Model.RecurrenceRuleParts;

public record Second
{
    public int Value { get; }

    public Second(int value)
    {
        if (value is < 0 or > 60)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
        
        Value = value;
    }
}
