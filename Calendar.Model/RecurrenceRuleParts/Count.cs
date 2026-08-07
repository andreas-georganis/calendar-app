namespace Calendar.API.Model.RecurrenceRuleParts;

public sealed class Count
{
    public int Value { get; }

    public Count(int value)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value,1);
        Value = value;
    }
}

