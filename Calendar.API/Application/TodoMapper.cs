namespace CalendarApp.API.Application;

public static class TodoMapper
{
    extension(Model.Todo todo)
    {
        internal Contracts.Todo ToContract()
        {
            ArgumentNullException.ThrowIfNull(todo);

            return new Contracts.Todo
            {
                Id = todo.Id,
                Title = todo.Title,
                Start = todo.Start.ToContract(),
                Due = todo.Due?.ToContract(),
                
            };
        }
    }
}
