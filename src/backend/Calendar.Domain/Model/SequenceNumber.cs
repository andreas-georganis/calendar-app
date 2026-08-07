namespace Calendar.Domain.Model;

public readonly record struct SequenceNumber
{
    public static SequenceNumber Zero => new(0);
    
    public int Value { get; }

    public SequenceNumber(int value)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value, 0, "Invalid sequence value");
        Value = value;
    }
    
    public SequenceNumber Increase() => new(Value + 1);
}
