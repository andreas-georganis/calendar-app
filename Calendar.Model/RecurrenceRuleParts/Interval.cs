namespace Calendar.API.Model.RecurrenceRuleParts;

public sealed class Interval
{
    public static Interval One() => new(1);
    
    public int Value { get; }

    public Interval(int value)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value,1);
        Value = value;
    }
}

