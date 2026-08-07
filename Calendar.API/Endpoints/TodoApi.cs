using CalendarApp.API.Application;
using CalendarApp.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CalendarApp.API.Endpoints;

internal static class TodoApi
{
    internal static RouteGroupBuilder MapTodoApi(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/todos");
        
        
        group.MapPost("/", async Task<Results<Created<Contracts.Todo>, NotFound>> (Contracts.Todo todoData, CalendarAppDbContext db, CurrentUser user) =>
        {
            // if (!MiniValidator.TryValidate(todoData, out var errors))
            // {
            //     return TypedResults.ValidationProblem(errors);
            // }
            
            if (await db.Calendars.FindAsync(todoData.CalendarId) is not { } calendar)
            {
                return TypedResults.NotFound();
            }

            Model.Todo todo = calendar.AddTodo(todoData.Title, todoData.Description, null, todoData.Priority,
                null, null, todoData.Location, null);
            
            await db.SaveChangesAsync();

            return TypedResults.Created($"/todos/{todo.Id}", todo.ToContract());
        });
        
        group.MapPut("/todos/{id}", null);

        return group;
    }
}
