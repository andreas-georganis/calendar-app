namespace Calendar.API.Model.RecurrenceRuleParts;

public record YearDay
{
    public YearDay(int value)
    {
        if (value is 0 || Math.Abs(value) > 366)
            throw new ArgumentOutOfRangeException(nameof(value));

        Value = value;
    }

    public int Value { get; }
}
