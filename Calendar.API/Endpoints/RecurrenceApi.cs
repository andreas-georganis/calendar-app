using Microsoft.AspNetCore.Http.HttpResults;

namespace CalendarApp.API.Endpoints;

public static class RecurrenceApi
{
    public static RouteGroupBuilder MapRecurrenceApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/recurrence-templates")
            .WithTags("RecurrenceTemplates");
        
        group.MapGet("/", Task<Ok<IEnumerable<Contracts.RecurrenceRuleTemplate>>> () =>
            {
                return default;
            })
            .RequireAuthorization()
            .WithSummary("Retrieves a set of fixed recurrence rules")
            .WithDescription("Retrieves a set of fixed recurrence rules");
        
        return group;
    }
}
