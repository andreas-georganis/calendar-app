namespace Calendar.API.Model.RecurrenceRuleParts;

public record Hour
{
    public int Value { get; }

    public Hour(int value)
    {
        if (value is < 0 or > 23)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
        
        
        Value = value;
    }
    
    public static implicit operator int(Hour hour) => hour.Value;
}
