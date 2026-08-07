using CalendarApp.API.Application;
using CalendarApp.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace CalendarApp.API.Endpoints;

internal static class CalendarApi
{
    internal static RouteGroupBuilder MapCalendarApi(this IEndpointRouteBuilder routes)
    { 
        var group = routes
            .MapGroup("/calendars")
            .WithTags("Calendars")
            .RequireAuthorization();

        group.MapGet("/", async Task<Ok<IEnumerable<Contracts.Calendar>>> (CalendarAppDbContext db, CurrentUser user) => 
            { 
                var calendars = await db.Calendars
                    .Where(c => c.UserId == user.Id).Select(Projections.Calendar).ToListAsync();

                return TypedResults.Ok<IEnumerable<Contracts.Calendar>>(calendars);
            })
            .WithDescription("Retrieves the user's calendars");

        group.MapGet("/{id:guid}", async Task<Results<Ok<Contracts.Calendar>, NotFound>> (Guid id, CalendarAppDbContext db, CurrentUser user) => 
            { 
                db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
                
                var calendar = await db.Calendars.FindAsync(id);
                    
                return calendar is not null ? TypedResults.Ok(calendar.ToContract()) : TypedResults.NotFound();
            })
            .WithDescription("Returns a calendar by its id");
        
        group.MapPost("/", async Task<Created<Contracts.Calendar>> (Contracts.Calendar calendarData, CalendarAppDbContext db, CurrentUser user) => 
            {
                // if (!MiniValidator.TryValidate(calendar, out var errors))
                // {
                //     return TypedResults.ValidationProblem(errors);
                // }
                
                var calendar = calendarData.ToModel(user.Id);
                
                db.Calendars.Add(calendar);
                await db.SaveChangesAsync();

                return TypedResults.Created($"/calendars/{calendar.Id}", calendar.ToContract());
            })
            .WithDescription("Creates a new calendar");
        
        group.MapPut("/{id:guid}", async Task<Results<NoContent, NotFound>> (Guid id, Contracts.Calendar calendarData, CalendarAppDbContext db) => 
            {
                // if (!MiniValidator.TryValidate(calendar, out var errors))
                // {
                //     return TypedResults.ValidationProblem(errors);
                // }
                
                var calendar = await db.Calendars.FindAsync(id);
                if (calendar is null)
                {
                    return TypedResults.NotFound();
                }

                calendar.Edit(calendarData.Name, calendarData.TimeZone.Id);
                
                await db.SaveChangesAsync();

                return TypedResults.NoContent(); 
            })
            .WithDescription("Edits an existing calendar");
        
        group.MapDelete("/{id:guid}", async Task<Results<NoContent, NotFound>> (Guid id, CalendarAppDbContext db) => 
            {
                var existing = await db.Calendars.FindAsync(id);
                if (existing is null)
                {
                    return TypedResults.NotFound();
                }

                db.Calendars.Remove(existing);
                await db.SaveChangesAsync();

                return TypedResults.NoContent(); 
            })
            .WithDescription("Deletes a calendar");
        
        return group;   
    }
}
