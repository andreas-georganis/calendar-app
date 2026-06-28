using System.ComponentModel.DataAnnotations;
using NodaTime;

namespace Calendar.Domain.Model;


public sealed class Calendar
{
    private readonly List<Todo> _todos = [];
    private readonly List<Event> _events = [];
    
    public Calendar(Guid userId, string name, DateTimeZone timeZone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentNullException.ThrowIfNull(timeZone);
        
        UserId = userId;
        Name = name;
        TimeZone = timeZone;
    }
    
    //[ViewOnly]
    public Guid Id { get; init; }
   
    //[ViewOnly]
    public Guid UserId { get; init; }
    
    [Required]
    public required string Name { get;  set; }
    
    [Required]
    public required DateTimeZone TimeZone { get;  set; }

    //[ViewOnly] 
    public IReadOnlyCollection<Link> Links { get; init; } = [];
    
    public void Edit(string name, string timeZone)
    {
        ArgumentNullException.ThrowIfNull(name);
        ArgumentNullException.ThrowIfNull(timeZone);
        
        Name = name;
        TimeZone = DateTimeZoneProviders.Tzdb[timeZone];
    }
    
    public IReadOnlyList<Event> Events
    {
        get => _events.AsReadOnly();
    }

    public IReadOnlyList<Todo> Todos
    {
        get=> _todos.AsReadOnly();
    }

    public Todo AddTodo(
        string title, 
        string? description, 
        DateTime start,
        DateTimeOrDuration due,
        Priority? priority,
        Alarm? alarm,
        RecurrenceRule? recurrenceRule,
        string? location,
        GeographicPosition geographicPosition)
    {
        var todo = new Todo(this.UserId, this.Id, title, description, start, due, priority, alarm, recurrenceRule,  location, geographicPosition);
        
        AddTodo(todo);

        return todo;
    }

    public Event AddEvent(
        string title,
        string? description,
        DateTime start,
        DateTimeOrDuration end,
        IEnumerable<Attendee>? attendees,
        Alarm? alarm,
        RecurrenceRule? recurrence,
        string? location,
        GeographicPosition geographicPosition)
    {
        var @event = new Event(this.UserId, this.Id, title, description, start, end,  alarm, recurrence, location, geographicPosition, attendees );
        
        AddEvent(@event);
        
        return @event;
    }
    
    internal void AddTodo(Todo todo)
    {
        _todos.Add(todo);
    }
    
    internal void AddEvent(Event @event)
    {
        _events.Add(@event);
    }
}
