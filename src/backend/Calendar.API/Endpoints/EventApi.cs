using Calendar.Domain.Model;
using Calendar.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using DateTime = System.DateTime;

namespace Calendar.API.Endpoints;

public static class EventApi
{
    public static RouteGroupBuilder MapEventApi(this RouteGroupBuilder app)
    {
        var group = app.MapGroup("/events")
            .WithTags("Events")
            .RequireAuthorization();
        
        // group.MapGet("/",
        //     async (Guid[] calendarIds, CalendarDbContext db, CurrentUser user, IOccurrenceCalculator occurrenceCalculator, DateTimeOffset? from,  DateTimeOffset? to) =>
        //     {
        //         to ??= DateTime.UtcNow; 
        //         from??= to.Value.AddDays(-7);
        //         
        //         var entries = await db.Entries.AsNoTracking()
        //             .Where(e => ((IEnumerable<Guid>)calendarIds).Contains(e.CalendarId) && e.UserId == user.Id)
        //             .Where(e =>
        //                 (e.Recurrence == null && (e.Start.Value >= from) && (e.Start.Value <= to))
        //                 ||
        //                 (e.Recurrence != null && 
        //                  (e.Start.Value <= to.Value) &&
        //                  (e.Recurrence.End!.Until == null ||
        //                   e.Recurrence.End.Until.Value >= from))
        //             )
        //             .ToListAsync();
        //
        //         var occurrences = entries.SelectMany(e => e.GetOccurrences(occurrenceCalculator, 
        //             new Model.DateTime(from.Value.UtcDateTime), new Model.DateTime(to.Value.UtcDateTime)));
        //
        //         return occurrences.Select(e => e.ToContract()).ToList();
        //     });
        
        app.MapPost("/calendars/{calendarId:guid}/events", async Task<Results<Created<Event>, NotFound>> (Domain.Model.CalendarId calendarId, Event eventData, CalendarDbContext db, UserId userId, CancellationToken cancellationToken) =>
        {
            if (await db.Calendars.FindAsync([calendarId], cancellationToken) is not { } calendar || calendar.UserId != userId)
            {
                return TypedResults.NotFound();
            }

            Event @event = calendar.AddEvent(eventData);
            
            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.Created($"/events/{@event.Id}", @event);
        });
        
        group.MapPut("{id:guid}", async Task<Results<NoContent, BadRequest<string>, NotFound>> (Domain.Model.EventId id, Event eventData, CalendarDbContext db, UserId userId, CancellationToken cancellationToken) =>
        {
            if (await db.Events.FindAsync([id], cancellationToken) is not { } @event || @event.UserId != userId)
            {
                return TypedResults.NotFound();
            }

            @event.Edit(eventData);
            
            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.NoContent();
        });
        
        group.MapDelete("{id:guid}", async Task<Results<NoContent, NotFound>> (Domain.Model.EventId id, CalendarDbContext db, UserId userId, CancellationToken cancellationToken) =>
        {
            if (await db.Events.FindAsync([id], cancellationToken) is not { } @event || @event.UserId != userId)
            {
                return TypedResults.NotFound();
            }

            db.Events.Remove(@event);
            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.NoContent();
        });

        return group;

        static API.Contracts.Event ToContract(Domain.Model.Event @event)
        {
            return new API.Contracts.Event
            {
                CalendarId = @event.CalendarId,
                Id = @event.Id,
                Start = @event.Start,
                End = @event.End,
                Duration = @event.Duration,
                Summary = @event.Summary,
                Description = @event.Description,
                Status = @event.Status,
                Location = @event.Location,
                RecurrenceRule = @event.RecurrenceRule,
                GeographicPosition = @event.GeographicPosition,
                Alarm = @event.Alarm,
                Classification = @event.Classification,
                Transparency = @event.Transparency,
                Created = @event.Created,
                LastModified = @event.LastModified,
                //Links = @event.Links.ToList()
            };
        }
    }
}
