using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Calendar.Domain.Model;



public sealed class Recurrence
{
    public RecurrenceRule? Rule { get; init; }
    
    public IReadOnlyCollection<System.DateTime>? ExceptionDateTimes { get; init; }
    
    public required IReadOnlyCollection<System.DateTime>? DateTimes { get; init; }
    
    public required IReadOnlyCollection<Period>? Periods { get; init; }
}


