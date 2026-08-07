namespace CalendarApp.API.Application;

internal static class JournalMapper
{
    extension(Model.Journal journal)
    {
        public Contracts.Journal ToContract()
        {
            return new Contracts.Journal();
        }
    }
    
    extension(Contracts.Journal journal)
    {
        public Model.Journal ToModel()
        {
            return new Model.Journal();
        }
    }
}
