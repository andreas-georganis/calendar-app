
namespace CalendarApp.API.Application;

public static class DateTimeMapper
{
    extension(Model.DateTime dateTime)
    {
        internal Contracts.DateTime ToContract()
        {
            ArgumentNullException.ThrowIfNull(dateTime);

            return new Contracts.DateTime()
            {
                Date = dateTime.Date,
                Time = dateTime.Time,
                TimeZone = dateTime.TimeZone
            };
        }
    }

    extension(Contracts.DateTime dateTime)
    {
        internal Model.DateTime ToModel()
        {
            ArgumentNullException.ThrowIfNull(dateTime);

            if (dateTime.Time is null)
            {
                return new Model.DateTime(dateTime.Date);
            }
            
            if (dateTime.TimeZone is null)
            {
                return new Model.DateTime(dateTime.Date, dateTime.Time.Value);
            }
            
            return new Model.DateTime(dateTime.Date, dateTime.Time.Value, dateTime.TimeZone);
        }
    }
}
