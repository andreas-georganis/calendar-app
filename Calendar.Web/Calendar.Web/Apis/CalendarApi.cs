using Microsoft.AspNetCore.Authentication;
using Yarp.ReverseProxy.Forwarder;
using Yarp.ReverseProxy.Transforms;

namespace Calendar.Web.Apis;

internal static class CalendarApi
{
    internal static IEndpointRouteBuilder MapCalendarApi(this IEndpointRouteBuilder app)
    {
        const string api = "http://calendar-app-api/api";

        foreach (var route in new[] { "calendars", "entries"})
        {
            app.MapAuthorizedForwarder($"{route}/{{**path}}", api);
        }

        return app;
    }
}
