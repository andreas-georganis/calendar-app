

namespace Calendar.API.Endpoints;

public static class TimeZoneApi
{
    internal static RouteGroupBuilder MapTimeZoneEndpoints(this RouteGroupBuilder app)
    {
        app.MapGet("/api/timezones", GetTimeZones)
            .WithName("GetTimeZones")
            .WithTags("TimeZones");
        

        static IResult GetTimeZones()
        {
            var timeZones = TimeZoneInfo.GetSystemTimeZones()
                .Select(tz => new Domain.Model.TimeZone { Id = tz.Id, DisplayName = BuildDisplayName(tz), Offset = tz.StandardName })
                .ToList();

            return Results.Ok(timeZones);
        }

        return app;

        static string BuildDisplayName(TimeZoneInfo tz)
        {
            // current offset (important for DST)
            var offset = tz.GetUtcOffset(DateTime.UtcNow);

            var sign = offset < TimeSpan.Zero ? "-" : "+";

            var formattedOffset =
                $"GMT{sign}{offset:hh\\:mm}";

            // take city from IANA id
            var city = tz.Id
                .Split('/')
                .Last()
                .Replace('_', ' ');

            return $"({formattedOffset}) {city}";
        }
    }
}
