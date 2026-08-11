using Calendar.Domain.Model;
using Calendar.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Calendar.API.Endpoints;

public static class EventApi
{
    public static RouteGroupBuilder MapEventApi(this RouteGroupBuilder app)
    {
        var group = app.MapGroup("/events")
            .WithTags("Events")
            .RequireAuthorization();

        var calendarGroup = app.MapGroup("/calendars/{calendarId:guid}/events")
            .WithTags("Events")
            .RequireAuthorization();

        calendarGroup.MapGet("/", async Task<Ok<IEnumerable<Contracts.Event>>> (Domain.Model.CalendarId calendarId, CalendarDbContext db, UserId userId, Domain.Model.DateTime from, Domain.Model.DateTime to, CancellationToken cancellationToken) =>
        {
            var events = await db.Events
                .Where(e => e.CalendarId == calendarId && e.UserId == userId)
                .Where(e =>
                        (e.RecurrenceRule == null &&
                         (e.Start <= to) &&
                         (
                             (e.End != null && e.End >= from)
                             || (e.End == null && e.Duration == null && e.Start >= from)
                             || (e.End == null && e.Duration != null && (e.Start + e.Duration >= from))
                         ))
                        ||
                        (e.RecurrenceRule != null && 
                         (e.Start <= to) &&
                         (e.RecurrenceRule.Until == null ||
                          e.RecurrenceRule.Until >= from))
                    )
                .ToListAsync(cancellationToken);

            var occurrences = events.SelectMany(e => e.GetOccurrences(from, to)).Select(e => ToContract(e));

            return TypedResults.Ok(occurrences);
        });
        
        calendarGroup.MapPost("/", async Task<Results<Created<Contracts.Event>, NotFound>> (Domain.Model.CalendarId calendarId, Event eventData, CalendarDbContext db, UserId userId, CancellationToken cancellationToken) =>
        {
            if (await db.Calendars.FindAsync([calendarId], cancellationToken) is not { } calendar || calendar.UserId != userId)
            {
                return TypedResults.NotFound();
            }

            Event @event = calendar.AddEvent(eventData);
            
            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.Created($"/events/{@event.Id}", ToContract(@event));
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
