namespace Calendar.API.Model.RecurrenceRuleParts;

public sealed record Ordinal
{
    public Ordinal(int value)
    {
        if (value is 0 || Math.Abs(value) > 53)
            throw new ArgumentOutOfRangeException(nameof(value));

        Value = value;
    }

    public int Value { get; set; }
}
