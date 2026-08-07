using System.ComponentModel.DataAnnotations;

namespace Calendar.Contracts;

public class GeographicPosition
{
    [Required]
    public required double Latitude { get; init; }
    
    [Required]
    public required double Longitude { get; init; }
}
