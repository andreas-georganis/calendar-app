namespace CalendarApp.API.Endpoints;

internal static class EventApi
{
    internal static RouteGroupBuilder MapEventApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/events");
        
        
        group.MapPut("/events/{id}", null);

        return group;
    }
}
