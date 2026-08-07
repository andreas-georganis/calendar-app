
using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace Calendar.Contracts;


public class Calendar
{
    [ViewOnly]
    public Guid Id { get; init; }
   
    [ViewOnly]
    public Guid UserId { get; init; }
    
    [Required]
    public required string Name { get; init; }
    
    [Required]
    public required DateTimeZone TimeZone { get; init; }

    [ViewOnly] 
    public IReadOnlyCollection<Link> Links { get; init; } = [];
}
