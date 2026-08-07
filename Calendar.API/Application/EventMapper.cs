using System.Collections.Immutable;


namespace CalendarApp.API.Application;

internal static class EventMapper
{
    extension(Model.Event @event)
    {
        public Contracts.Event ToContract()
        {
            ArgumentNullException.ThrowIfNull(@event);

            return new Contracts.Event
            {
                Id = @event.Id,
                CalendarId = @event.CalendarId,
                UserId = @event.UserId,
                Title = @event.Title,
                Description = @event.Description,
                Status =  @event.Status,
                Start = @event.Interval.Start.ToContract(),
                
                End = @event.Interval?.End?.ToContract(),
                Location = @event.Location,
                GeographicPosition = @event.GeographicPosition is null ? null : new Contracts.GeographicPosition(){ Latitude = @event.GeographicPosition.Latitude, Longitude = @event.GeographicPosition.Longitude},
                Attendees = @event.Attendees?.Select(a=> new Contracts.Attendee()
                {
                    Name = a.CommonName,
                    Rsvp = a.Rsvp,
                    Role = a.Role
                }).ToImmutableHashSet(),
                RecurrenceRule = @event.Recurrence?.ToContract(),
                Created =  @event.Created.ToContract(),
                LastModified =  @event.LastModified?.ToContract(),
            };
        }
    }

    extension(Contracts.Attendee attendee)
    {
        Model.Attendee ToModel()
        {
            return new Model.Attendee(attendee.Name, attendee.Email, attendee.Rsvp, attendee.Role, attendee.ParticipationStatus);
        }
    }
}
