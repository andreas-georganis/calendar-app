namespace Calendar.Model;

public readonly struct DateTime
{
    public DateTime()
    {
        
    }
}

public sealed record Utc(Instant Value)
{
    public static Utc Now()
        => Now(SystemClock.Instance);
    
    public static Utc Now(IClock clock)
    {
        var instant = clock.GetCurrentInstant();
       
        return new Utc(instant);
    }
}
