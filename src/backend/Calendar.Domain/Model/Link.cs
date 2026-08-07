namespace Calendar.Domain.Model;

public sealed class Link
{
    public required string Href { get; set; }
    public required string Rel { get; set; }
    public required string Method { get; set; }
}
