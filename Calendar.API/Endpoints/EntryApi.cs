using CalendarApp.API.Application;
using CalendarApp.API.Infrastructure;
using CalendarApp.API.Model;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using DateTime = System.DateTime;

namespace CalendarApp.API.Endpoints;

internal static class EntryApi
{
    internal static RouteGroupBuilder MapCalendarEntryApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/entries")
            .WithTags("Calendar Entries")
            .RequireAuthorization();

        group.MapGet("/",
            async (Guid[] calendarIds, CalendarAppDbContext db, CurrentUser user, IOccurrenceCalculator occurrenceCalculator, DateTimeOffset? from,  DateTimeOffset? to) =>
            {
                to ??= DateTime.UtcNow; 
                from??= to.Value.AddDays(-7);
                
                var entries = await db.Entries.AsNoTracking()
                    .Where(e => ((IEnumerable<Guid>)calendarIds).Contains(e.CalendarId) && e.UserId == user.Id)
                    .Where(e =>
                        (e.Recurrence == null && (e.Start.Value >= from) && (e.Start.Value <= to))
                        ||
                        (e.Recurrence != null && 
                         (e.Start.Value <= to.Value) &&
                         (e.Recurrence.End!.Until == null ||
                          e.Recurrence.End.Until.Value >= from))
                    )
                    .ToListAsync();
        
                var occurrences = entries.SelectMany(e => e.GetOccurrences(occurrenceCalculator, 
                    new Model.DateTime(from.Value.UtcDateTime), new Model.DateTime(to.Value.UtcDateTime)));

                return occurrences.Select(e => e.ToContract()).ToList();
            });
        
        group.MapPost("/", async Task<Results<Created<Contracts.Entry>, NotFound>> (Contracts.Entry entryData, CalendarAppDbContext db, CurrentUser user) =>
        {
            // if (!MiniValidator.TryValidate(todoData, out var errors))
            // {
            //     return TypedResults.ValidationProblem(errors);
            // }
            
            if (await db.Calendars.FindAsync(entryData.CalendarId) is not { } calendar)
            {
                return TypedResults.NotFound();
            }

            Model.Entry entry = entryData switch
            {
                Contracts.Todo todo => calendar.AddTodo(todo.Title, todo.Description, null,null,  todo.Priority,
                    null, null, todo.Location, null, null),
                Contracts.Event @event => calendar.AddEvent(@event.Title, @event.Description, null, null,
                    null, null, null, @event.Location,null, null),
                _ => throw new InvalidOperationException("Invalid entry type")
            };
            
            await db.SaveChangesAsync();

            return TypedResults.Created($"/todos/{entry.Id}", entry.ToContract());
        });
        
        group.MapPut("/{id:guid}/complete", async Task<Results<NoContent, NotFound>> (Guid id, CalendarAppDbContext db) =>
        {
            if (await db.Todos.FindAsync(id) is not { } todo)
            {
                return TypedResults.NotFound();
            }

            todo.Complete();
            
            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        });
        
        group.MapPut("{id:guid}", async Task<Results<NoContent, BadRequest<string>, NotFound>> (Guid id, Contracts.Entry entryData, CalendarAppDbContext db) =>
        {
            // if (!MiniValidator.TryValidate(todoData, out var errors))
            // {
            //     return TypedResults.ValidationProblem(errors);
            // }
            
            if (await db.Entries.FindAsync(id) is not { } entry)
            {
                return TypedResults.NotFound();
            }

            (bool success, bool mismatch) = (entry, entryData) switch
            {
                (Model.Todo todo, Contracts.Todo todoData) => (todo.Edit(todoData.Title, todoData.Description, null,
                    todoData.Priority), false),
                (Model.Event @event, Contracts.Event eventData) => (@event.Edit(eventData.Title,
                    eventData.Description, null, eventData.Location, null), false),
                (Model.Todo, Contracts.Event) or (Model.Event, Contracts.Todo) => (false, true),
                _ => (false, false)
            };

            if (mismatch)
            {
                return TypedResults.BadRequest("Payload mismatch");
            }
            
            if (!success)
            {
                return TypedResults.BadRequest("Failed to edit entry");
            }
            
            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        });
        
        group.MapDelete("{id:guid}", async Task<Results<NoContent, NotFound>> (Guid id, CalendarAppDbContext db) =>
        {
            if (await db.Entries.FindAsync(id) is not { } todo)
            {
                return TypedResults.NotFound();
            }

            db.Entries.Remove(todo);
            await db.SaveChangesAsync();

            return TypedResults.NoContent();
        });
        
       return group;
    }

    
}
