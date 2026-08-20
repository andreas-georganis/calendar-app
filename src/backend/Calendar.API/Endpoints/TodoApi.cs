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

        calendarGroup.MapGet("/", async Task<Ok<IEnumerable<Contracts.Todo>>> (Domain.Model.CalendarId calendarId, CalendarDbContext db, UserId userId, Domain.Model.CalDateTime from, Domain.Model.CalDateTime to, CancellationToken cancellationToken) =>
        {
            var todos = await db.Todos
                .Where(t => t.CalendarId == calendarId && t.UserId == userId)
                .Where(t =>
                        (t.RecurrenceRule == null &&
                         (t.Start <= to) &&
                         (
                             (t.Due != null && t.Due >= from)
                             || (t.Due == null && t.Duration == null && t.Start >= from)
                             || (t.Due == null && t.Duration != null && (t.Start + t.Duration.Value >= from))
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
        
        calendarGroup.MapPost("/", async Task<Results<Created<Contracts.Todo>, NotFound>> (Domain.Model.CalendarId calendarId, Contracts.Todo todoData, CalendarDbContext db, UserId userId, CancellationToken cancellationToken) =>
        {
            if (await db.Calendars.FindAsync([calendarId], cancellationToken) is not { } calendar || calendar.UserId != userId)
            {
                return TypedResults.NotFound();
            }

            var todo = new Domain.Model.Todo(
                userId,
                calendarId,
                todoData.Id,
                todoData.Summary,
                todoData.Description,
                todoData.Start,
                todoData.Due,
                todoData.Duration,
                todoData.Priority,
                todoData.Alarm,
                todoData.RecurrenceRule,
                todoData.RecurrencePeriods,
                todoData.RecurrenceDates,
                todoData.ExceptionDates,
                todoData.Location,
                todoData.GeographicPosition,
                todoData.Classification,
                SystemClock.Instance.GetCurrentInstant()
            );

            _ = calendar.AddTodo(todo);
            
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
        
        group.MapPut("{id:guid}", async Task<Results<NoContent, BadRequest<string>, NotFound>> (TodoId id, Contracts.Todo todoData, CalendarDbContext db, UserId userId, CancellationToken cancellationToken) =>
        {
            if (await db.Todos.FindAsync([id], cancellationToken) is not { } todo || todo.UserId != userId)
            {
                return TypedResults.NotFound();
            }

            todo.Edit(
                summary: todoData.Summary,
                description: todoData.Description,
                location: todoData.Location,
                geographicPosition: todoData.GeographicPosition,
                alarm: todoData.Alarm,
                recurrenceRule: todoData.RecurrenceRule,
                priority: todoData.Priority
            );
            
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
    }

    extension(Domain.Model.Todo todo)
    {
        public Contracts.Todo ToContract()
        {
            return new Contracts.Todo
            {
                Id = todo.Id,
                Summary = todo.Summary,
                Description = todo.Description,
                Start = todo.Start,
                Due = todo.Due,
                Duration = todo.Duration,
                Priority = todo.Priority,
                Alarm = todo.Alarm,
                RecurrenceRule = todo.RecurrenceRule,
                RecurrencePeriods = todo.RecurrencePeriods,
                RecurrenceDates = todo.RecurrenceDates,
                ExceptionDates = todo.ExceptionDates,
                Location = todo.Location,
                GeographicPosition = todo.GeographicPosition,
                Classification = todo.Classification,
                Status = todo.Status,
                Completed = todo.Completed
            };
        }
    }

    extension(Contracts.Todo todo)
    {
        public Domain.Model.Todo ToDomain(UserId userId, CalendarId calendarId, IClock clock)
        {
            return new Domain.Model.Todo(
                userId,
                calendarId,
                todo.Id,
                todo.Summary,
                todo.Description,
                todo.Start,
                todo.Due,
                todo.Duration,
                todo.Priority,
                todo.Alarm,
                todo.RecurrenceRule,
                todo.RecurrencePeriods,
                todo.RecurrenceDates,
                todo.ExceptionDates,
                todo.Location,
                todo.GeographicPosition,
                todo.Classification,
                clock.GetCurrentInstant()
            );
        }
    }
}
