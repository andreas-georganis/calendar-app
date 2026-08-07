using System.Linq.Expressions;
using CalendarApp.Contracts;

namespace CalendarApp.API.Application;

public static class Projections
{
    public static Expression<Func<Model.Calendar, Contracts.Calendar>> Calendar =>
        calendar => new Calendar()
        {
            Id = calendar.Id,
            UserId = calendar.UserId,
            Name = calendar.Name,
            TimeZone = calendar.TimeZone
        };
    
  
}
