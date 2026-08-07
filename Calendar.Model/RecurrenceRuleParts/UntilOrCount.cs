namespace Calendar.API.Model.RecurrenceRuleParts;

public sealed class UntilOrCount
{
    public static UntilOrCount Forever => new(null, null);

    public static UntilOrCount On(DateTime start, DateTime value)
    {
        return new(value, null);
    }

    public static UntilOrCount After(Count value)
        => new(null, value);
    
    private UntilOrCount(DateTime? until, Count? count)
    {
        Until = until;
        Count = count;
    }

    public DateTime? Until { get; }
    public Count? Count { get; }
}
