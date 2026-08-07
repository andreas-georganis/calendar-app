
namespace Calendar.API.Model;

public sealed record TimeInterval
{
    public TimeInterval(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }
    
    public DateTime Start { get; }
    public DateTime End { get; }
    //public Duration? Duration { get; }
    
}
