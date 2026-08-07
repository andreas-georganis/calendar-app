using NodaTime;

namespace Calendar.Domain.Model;

public sealed class FreeBusy
{

    public FreeBusy()
    {
    }

    public required Instant Start { get; init; }
    
    public required Instant End { get; init; }

}
