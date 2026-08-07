namespace CalendarApp.API.Application;

public static class EntryMapper
{
    extension(Model.Entry entry)
    {
        public Contracts.Entry ToContract()
        {
            return entry switch
            {
                Model.Todo todo => todo.ToContract(),
                Model.Event @event => @event.ToContract(),
                Model.Journal journal => journal.ToContract(),
                _ => throw new ArgumentException("Invalid entry type")
            };
        }
    }
}
