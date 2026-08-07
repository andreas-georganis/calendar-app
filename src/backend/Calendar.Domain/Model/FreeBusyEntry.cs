namespace Calendar.Domain.Model;

public sealed class FreeBusyEntry
{
    public FreeBusyEntry(FreeBusyType fbType, Period period)
    {
        FbType = fbType;
        Period = period;
    }

    public FreeBusyType FbType { get; }
    public Period Period { get; }
}
