namespace Calendar.Contracts;

public sealed class TimeZone
{
    public required string Id { get; set; }
    public string DisplayName { get; set; }
    public string Offset { get; set; }
}
