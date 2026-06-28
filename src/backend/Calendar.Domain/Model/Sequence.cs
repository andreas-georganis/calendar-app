namespace Calendar.Domain.Model;

public readonly record struct Sequence
{
    public static Sequence Zero => new Sequence(0);
    
    public int Number { get; }

    public Sequence(int number)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(number, 0, "Invalid sequence number");
        Number = number;
    }
    
    public Sequence Increase() => new Sequence(Number + 1);
}
