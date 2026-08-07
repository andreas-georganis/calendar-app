
using Calendar.Domain.Model;
using Calendar.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Calendar.API.Endpoints;

internal static class CalendarApi
{
    internal static RouteGroupBuilder MapCalendarApi(this IEndpointRouteBuilder routes)
    { 
        var group = routes
            .MapGroup("/calendars")
            .WithTags("Calendars")
            .RequireAuthorization();

        group.MapGet("/", async Task<Ok<IEnumerable<Contracts.Calendar>>> (CalendarDbContext db, UserId userId, CancellationToken cancellationToken) =>
        { 
            var calendars = await db.Calendars
            .Where(c => c.UserId == userId)
            .Select(c => new Contracts.Calendar
            {
                Id = c.Id,
                Name = c.Name,
                TimeZone = c.TimeZone
            }).ToListAsync(cancellationToken);

            return TypedResults.Ok<IEnumerable<Contracts.Calendar>>(calendars);
        })
        .WithDescription("Retrieves the user's calendars");

        group.MapGet("/{id:guid}", async Task<Results<Ok<Contracts.Calendar>, NotFound>> (CalendarId id, CalendarDbContext db, UserId userId, CancellationToken cancellationToken) => 
        { 
            db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            
            var calendar = await db.Calendars.FindAsync([id], cancellationToken);
            if (calendar?.UserId != userId)
            {
                return TypedResults.NotFound();
            }
                
            return calendar is not null ? TypedResults.Ok<Contracts.Calendar>(new Contracts.Calendar
            {
                Id = calendar.Id,
                Name = calendar.Name,
                TimeZone = calendar.TimeZone
            }) : TypedResults.NotFound();
        })
        .WithDescription("Returns a calendar by its id");
        
        group.MapPost("/", async Task<Created<Contracts.Calendar>> (Contracts.Calendar calendarData, UserId userId, CalendarDbContext db, CancellationToken cancellationToken) => 
        {
            var calendar = new Domain.Model.Calendar(userId, calendarData.Id, calendarData.Name, calendarData.TimeZone);

            db.Calendars.Add(calendar);
            await db.SaveChangesAsync(cancellationToken);
            return TypedResults.Created($"/calendars/{calendar.Id}", new Contracts.Calendar
            {
                Id = calendar.Id,
                Name = calendar.Name,
                TimeZone = calendar.TimeZone
            });
        })
        .WithDescription("Creates a new calendar");
        
        group.MapPut("/{id:guid}", async Task<Results<NoContent, NotFound>> (CalendarId id, Contracts.Calendar calendarData, CalendarDbContext db, UserId userId, CancellationToken cancellationToken) => 
        {
            var calendar = await db.Calendars.FindAsync([id], cancellationToken);
            if (calendar is null)
            {
                return TypedResults.NotFound();
            }

            if (calendar.UserId != userId)
            {
                return TypedResults.NotFound();
            }

            calendar.Edit(calendarData.Name, calendarData.TimeZone);
            
            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.NoContent(); 
        })
        .WithDescription("Edits an existing calendar");
        
        group.MapDelete("/{id:guid}", async Task<Results<NoContent, NotFound>> (CalendarId id, CalendarDbContext db, UserId userId, CancellationToken cancellationToken) => 
        {
            var existing = await db.Calendars.FindAsync([id], cancellationToken);

            if (existing is null)
            {
                return TypedResults.NotFound();
            }

            if (existing.UserId != userId)
            {
                return TypedResults.NotFound();
            }

            db.Calendars.Remove(existing);
            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.NoContent(); 
        })
        .WithDescription("Deletes a calendar");
        
        return group;   
    }

    
}