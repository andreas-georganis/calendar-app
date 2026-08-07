namespace Calendar.API.Model.RecurrenceRuleParts;

public record Month
{
    public Month(int value)
    {
        if (value is < 0 or > 12)
        {
            throw new ArgumentOutOfRangeException(nameof(value));
        }
       
        Value = value;
    }

    public int Value { get; }
}
