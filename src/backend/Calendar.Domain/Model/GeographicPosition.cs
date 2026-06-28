namespace Calendar.Domain.Model;

public readonly record struct GeographicPosition
{
    public required double Latitude { get; init; }
    
    public required double Longitude { get; init; }
}
