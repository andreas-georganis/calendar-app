using NodaTime;

namespace Calendar.Domain.Model;


public sealed class Calendar
{
    private readonly List<Todo> _todos = [];
    private readonly List<Event> _events = [];
    
    public Calendar(UserId userId, CalendarId id, Name name, DateTimeZone timeZone)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(timeZone);

        UserId = userId;
        Id = id;
        Name = name;
        TimeZone = timeZone;
    }

    public UserId UserId { get; init; }
    
    public CalendarId Id { get; init; }
    
    public Name Name { get;  private set; }
    
    public DateTimeZone TimeZone { get;  private set; }
    
    public void Edit(Name name, DateTimeZone timeZone)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(timeZone);
        
        Name = name;
        TimeZone = timeZone;
    }
    
    public IReadOnlyList<Event> Events
    {
        get => _events.AsReadOnly();
    }

    public IReadOnlyList<Todo> Todos
    {
        get=> _todos.AsReadOnly();
    }

    // public Todo AddTodo(
    //     string title, 
    //     string? description, 
    //     DateTime start,
    //     DateTimeOrDuration due,
    //     Priority? priority,
    //     Alarm? alarm,
    //     RecurrenceRule? recurrenceRule,
    //     RecurrenceDateTimes? recurrenceDates,
    //     ExceptionDateTimes? exceptionDates,
    //     string? location,
    //     GeographicPosition geographicPosition, Instant created)
    // {
    //     var todo = new Todo(this.UserId, this.Id, title, description, start, due, priority, alarm, recurrenceRule, recurrenceDates, exceptionDates, location, geographicPosition, created);
    //     
    //     AddTodo(todo);
    //
    //     return todo;
    // }
    //
    // public Event AddEvent(
    //     string summary,
    //     string? description,
    //     DateTime start,
    //     DateTimeOrDuration end,
    //     IEnumerable<Attendee>? attendees,
    //     Alarm? alarm,
    //     RecurrenceRule? recurrence,
    //     RecurrenceDateTimes? recurrenceDates,
    //     ExceptionDateTimes? exceptionDates,     
    //     string? location,
    //     GeographicPosition geographicPosition, 
    //     Instant created)
    // {
    //     var @event = new Event(this.UserId, this.Id, summary, description, start, end,  alarm, recurrence, recurrenceDates, exceptionDates, location, geographicPosition, attendees, created);
    //     
    //     AddEvent(@event);
    //     
    //     return @event;
    // }
    
    public Todo AddTodo(Todo todo)
    {
        _todos.Add(todo);
        return todo;
    }
    
    public Event AddEvent(Event @event)
    {
        _events.Add(@event);
        return @event;
    }
}
