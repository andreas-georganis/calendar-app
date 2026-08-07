namespace Calendar.API.Model.RecurrenceRuleParts;

public record MonthDay
{
    public int Value { get; }

    public MonthDay(int value)
    {
        if (value is 0 || Math.Abs(value) > 31)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
        
        Value = value;
    }
}
