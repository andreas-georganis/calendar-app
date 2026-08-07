using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace Calendar.Contracts;

public sealed class Period
{
    [Required]
    public required DateTime Start { get; init; }
        
    public DateTime? End { get; init; }
    
    public Duration? Duration { get; init; }
}
