namespace Calendar.API.Model.RecurrenceRuleParts;

public record WeekNo
{
    public int Value { get; }

    public WeekNo(int value)
    {
        if (value is 0 || Math.Abs(value) > 53)
            throw new ArgumentOutOfRangeException(nameof(value));
        
        Value = value;
    }
    
    public static implicit operator int(WeekNo weekNo) => weekNo.Value;
}
