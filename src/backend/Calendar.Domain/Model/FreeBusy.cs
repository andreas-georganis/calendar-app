using NodaTime;

namespace Calendar.Domain.Model;

public enum FreeBusyType
{
    Free,
    Busy,
    BusyTentative,
    BusyUnavailable
}


public sealed class FreeBusy
{
    //[ViewOnly]
    public required Instant Start { get; init; }
    
    //[ViewOnly]
    public required Instant End { get; init; }
    
    public FreeBusyType? Type { get; init; }
}
