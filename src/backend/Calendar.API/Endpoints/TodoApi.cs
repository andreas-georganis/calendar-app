using Calendar.Domain.Model;
using Calendar.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using NodaTime;
using Microsoft.EntityFrameworkCore;

namespace Calendar.API.Endpoints;

public static class TodoApi
{
    public static RouteGroupBuilder MapTodoApi(this RouteGroupBuilder app)
    {
        var group = app.MapGroup("/todos")
            .WithTags("Todos")
            .RequireAuthorization();

        var calendarGroup = app.MapGroup("/calendars/{calendarId:guid}/todos")
            .WithTags("Todos")
            .RequireAuthorization();

        calendarGroup.MapGet("/", async Task<Ok<IEnumerable<Contracts.Todo>>> (Domain.Model.CalendarId calendarId, CalendarDbContext db, UserId userId, Domain.Model.DateTime from, Domain.Model.DateTime to, CancellationToken cancellationToken) =>
        {
            var todos = await db.Todos
                .Where(t => t.CalendarId == calendarId && t.UserId == userId)
                .Where(t =>
                        (t.RecurrenceRule == null &&
                         (t.Start <= to) &&
                         (
                             (t.Due != null && t.Due >= from)
                             || (t.Due == null && t.Duration == null && t.Start >= from)
                             || (t.Due == null && t.Duration != null && (t.Start + t.Duration >= from))
                         ))
                        ||
                        (t.RecurrenceRule != null && 
                         (t.Start <= to) &&
                         (t.RecurrenceRule.Until == null ||
                          t.RecurrenceRule.Until >= from))
                    )
                .ToListAsync(cancellationToken);

            var occurrences = todos.SelectMany(t => t.GetOccurrences(from, to)).Select(t => ToContract(t));

            return TypedResults.Ok(occurrences);
        });
        
        calendarGroup.MapPost("/", async Task<Results<Created<Contracts.Todo>, NotFound>> (Domain.Model.CalendarId calendarId, Todo todoData, CalendarDbContext db, UserId userId, CancellationToken cancellationToken) =>
        {
            if (await db.Calendars.FindAsync([calendarId], cancellationToken) is not { } calendar || calendar.UserId != userId)
            {
                return TypedResults.NotFound();
            }

            Todo todo = calendar.AddTodo(todoData);
            
            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.Created($"/todos/{todo.Id}", ToContract(todo));
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
