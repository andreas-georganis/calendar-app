using NodaTime;

namespace CalendarApp.API.Application;

internal static class CalendarMapper
{
    extension(Model.Calendar calendar)
    {
        internal Contracts.Calendar ToContract()
        {
            ArgumentNullException.ThrowIfNull(calendar);
            
            return new Contracts.Calendar
            {
                Id = calendar.Id,
                UserId = calendar.UserId,
                Name = calendar.Name,
                TimeZone = calendar.TimeZone
            };
        }
    }

    extension(Contracts.Calendar calendar)
    {
        internal Model.Calendar ToModel(Guid userId)
        {
            ArgumentNullException.ThrowIfNull(calendar);
            
            return new Model.Calendar(userId, calendar.Name, calendar.TimeZone);
        }
    }
}
