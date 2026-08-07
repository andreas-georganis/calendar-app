using Calendar.Domain.Model;
using Calendar.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using NodaTime;

namespace Calendar.API.Endpoints;

public static class TodoApi
{
    public static RouteGroupBuilder MapTodoApi(this RouteGroupBuilder app)
    {
        var group = app.MapGroup("/todos")
            .WithTags("Todos")
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
        
        app.MapPost("/calendars/{calendarId:guid}/todos", async Task<Results<Created<Todo>, NotFound>> (Domain.Model.CalendarId calendarId, Todo todoData, CalendarDbContext db, UserId userId, CancellationToken cancellationToken) =>
        {
            if (await db.Calendars.FindAsync([calendarId], cancellationToken) is not { } calendar || calendar.UserId != userId)
            {
                return TypedResults.NotFound();
            }

            Todo todo = calendar.AddTodo(todoData);
            
            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.Created($"/todos/{todo.Id}", todo);
        });
        
        group.MapPut("/{id:guid}/complete", async Task<Results<NoContent, NotFound>> (TodoId id, CalendarDbContext db, IClock clock, UserId userId, CancellationToken cancellationToken) =>
        {
            if (await db.Todos.FindAsync([id], cancellationToken) is not { } todo || todo.UserId != userId)
            {
                return TypedResults.NotFound();
            }

            todo.Complete(clock);
            
            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.NoContent();
        });
        
        group.MapPut("{id:guid}", async Task<Results<NoContent, BadRequest<string>, NotFound>> (TodoId id, Todo todoData, CalendarDbContext db, UserId userId, CancellationToken cancellationToken) =>
        {
            if (await db.Todos.FindAsync([id], cancellationToken) is not { } todo || todo.UserId != userId)
            {
                return TypedResults.NotFound();
            }

            todo.Edit(todoData);
            
            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.NoContent();
        });
        
        group.MapDelete("{id:guid}", async Task<Results<NoContent, NotFound>> (TodoId id, CalendarDbContext db, UserId userId, CancellationToken cancellationToken) =>
        {
            if (await db.Todos.FindAsync([id], cancellationToken) is not { } todo || todo.UserId != userId)
            {
                return TypedResults.NotFound();
            }

            db.Todos.Remove(todo);
            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.NoContent();
        });

        return group;

        static Contracts.Todo ToContract(Domain.Model.Todo todo)
        {
            return new Contracts.Todo
            {
                CalendarId = todo.CalendarId,
                Id = todo.Id,
                Start = todo.Start,
                Due = todo.Due,
                Duration = todo.Duration,
                Summary = todo.Summary,
                Description = todo.Description,
                Status = todo.Status,
                Location = todo.Location,
                GeographicPosition = todo.GeographicPosition,
                RecurrenceRule = todo.RecurrenceRule,
                Priority = todo.Priority,
                Alarm = todo.Alarm,
                Classification = todo.Classification,
                Completed = todo.Completed,
                Created = todo.Created,
                LastModified = todo.LastModified
            };
        }
    }
}
