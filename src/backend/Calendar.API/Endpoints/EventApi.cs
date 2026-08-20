using Calendar.Domain.Model;
using Calendar.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

using NodaTime;

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

        calendarGroup.MapGet("/", async Task<Ok<IEnumerable<Contracts.Event>>> (Domain.Model.CalendarId calendarId, CalendarDbContext db, UserId userId, Domain.Model.CalDateTime from, Domain.Model.CalDateTime to, CancellationToken cancellationToken) =>
        {
            var events = await db.Events
                .Where(e => e.CalendarId == calendarId && e.UserId == userId)
                .Where(e =>
                        (e.RecurrenceRule == null &&
                         (e.Start <= to) &&
                         (
                             (e.End != null && e.End >= from)
                             || (e.End == null && e.Duration == null && e.Start >= from)
                             || (e.End == null && e.Duration != null && (e.Start + e.Duration.Value >= from))
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
        
        calendarGroup.MapPost("/", async Task<Results<Created<Contracts.Event>, NotFound>> (Domain.Model.CalendarId calendarId, API.Contracts.Event eventData, CalendarDbContext db, UserId userId, CancellationToken cancellationToken) =>
        {
            if (await db.Calendars.FindAsync([calendarId], cancellationToken) is not { } calendar || calendar.UserId != userId)
            {
                return TypedResults.NotFound();
            }

            var @event = eventData.ToDomain(userId, calendarId);
            _ = calendar.AddEvent(@event);
            
            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.Created($"/events/{@event.Id}", ToContract(@event));
        });
        
        group.MapPut("{id:guid}", async Task<Results<NoContent, BadRequest<string>, NotFound>> (Domain.Model.EventId id, Contracts.Event eventData, CalendarDbContext db, UserId userId, CancellationToken cancellationToken) =>
        {
            if (await db.Events.FindAsync([id], cancellationToken) is not { } @event || @event.UserId != userId)
            {
                return TypedResults.NotFound();
            }

            @event.Edit(
                summary: eventData.Summary,
                description: eventData.Description,
                location: eventData.Location,
                geographicPosition: eventData.GeographicPosition,
                alarm: eventData.Alarm,
                recurrenceRule: eventData.RecurrenceRule
            );
            
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

        
    }

    extension(Domain.Model.Event @event)
    {
        public Contracts.Event ToContract()
        {
            return new Contracts.Event
            {
                Id = @event.Id,
                CalendarId = @event.CalendarId,
                Summary = @event.Summary,
                Description = @event.Description,
                Start = @event.Start,
                End = @event.End,
                Duration = @event.Duration,
                Alarm = @event.Alarm,
                RecurrenceRule = @event.RecurrenceRule,
                RecurrencePeriods = @event.RecurrencePeriods,
                RecurrenceDates = @event.RecurrenceDates,
                ExceptionDates = @event.ExceptionDates,
                Location = @event.Location,
                GeographicPosition = @event.GeographicPosition,
                Attendees = @event.Attendees?.ToList(),
                Created = @event.Created
            };
        }
    }

    extension(Contracts.Event @event)
    {
        public Domain.Model.Event ToDomain(UserId id, CalendarId calendarId)
        {
            return new Domain.Model.Event(
                userId: id,
                calendarId: calendarId,
                id: @event.Id,
                summary: @event.Summary,
                description: @event.Description,
                start: @event.Start,
                end: @event.End,
                duration: @event.Duration,
                alarm: @event.Alarm,
                recurrenceRule: @event.RecurrenceRule,
                recurrencePeriods: null, // TODO
                recurrenceDates: null, // TODO
                exceptionDates: null, // TODO
                location: @event.Location,
                geographicPosition: @event.GeographicPosition,
                attendees: null, // TODO
                created: @event.Created
            );
        }
    } 
}
