using System.ComponentModel.DataAnnotations;
using Calendar.Domain.Model;
using NodaTime;

namespace Calendar.API.Contracts;

public sealed class Calendar
{
    public CalendarId Id { get; init; } = CalendarId.New();
       
    [Required]
    public required CalendarName Name { get; init; }
    
    [Required]
    public required DateTimeZone TimeZone { get; init; }
}
