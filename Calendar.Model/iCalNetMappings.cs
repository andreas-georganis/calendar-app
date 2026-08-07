using System.Collections.Immutable;
using Calendar.API.Model.RecurrenceRuleParts;
using Calendar.Contracts;
using Ical.Net;
using Ical.Net.CalendarComponents;
using NodaTime;
using NodaTime.Extensions;
using AlarmAction = Calendar.Contracts.AlarmAction;
using EventStatus = Calendar.Contracts.EventStatus;
using Interval = Calendar.API.Model.RecurrenceRuleParts.Interval;
using Period = Ical.Net.DataTypes.Period;
using TodoStatus = Calendar.Contracts.TodoStatus;
using TriggerRelation = Calendar.Contracts.TriggerRelation;
using WeekDay = Calendar.API.Model.RecurrenceRuleParts.WeekDay;

namespace Calendar.API.Model;

using Interval = global::Calendar.API.Model.RecurrenceRuleParts.Interval;

public static class iCalNetMappings
{
    extension(Ical.Net.DataTypes.CalDateTime dateTime)
    {
        Model.DateTime ToModel()
        {
            ArgumentNullException.ThrowIfNull(dateTime, nameof(dateTime));

            var date = LocalDate.FromDateOnly(dateTime.Date);
            LocalTime? time = dateTime.Time.HasValue? LocalTime.FromTimeOnly(dateTime.Time.Value): null;
            var timeZone = dateTime.TzId is not null ? NodaTime.DateTimeZoneProviders.Tzdb[dateTime.TzId] : null;

            var dt = (date, time, timeZone) switch
            {
                { time:null } => new Model.DateTime(date),
                {time:not null, timeZone:null} => new Model.DateTime(date, time.Value),
                _ => new Model.DateTime(date, time.Value, timeZone)
            };

            return dt;
        }
    }
    
    
    extension(Model.DateTime dateTime)
    {
        public Ical.Net.DataTypes.CalDateTime ToIcal()
        {
            ArgumentNullException.ThrowIfNull(dateTime, nameof(dateTime));

            return new Ical.Net.DataTypes.CalDateTime(dateTime.Date, dateTime.Time, dateTime.TimeZone?.Id);
        }
    }

    extension(Model.Duration duration)
    {
        public Ical.Net.DataTypes.Duration ToIcal()
        {
            if (duration == default)
            {
                throw new ArgumentNullException(nameof(duration));
            }
            
            return new Ical.Net.DataTypes.Duration(duration.Weeks, duration.Days,  duration.Hours, duration.Minutes, duration.Seconds);
        }
    }
    
    extension(Ical.Net.DataTypes.Duration? duration)
    {
        public Model.Duration ToModel()
        {
            if (duration is null)
            {
                throw new ArgumentNullException(nameof(duration));
            }
            
            return new Model.Duration(duration.Value.Weeks, duration.Value.Days,  duration.Value.Hours, duration.Value.Minutes, duration.Value.Seconds);
        }
    }
    
    extension(Model.GeographicPosition? geolocation)
    {
        public Ical.Net.DataTypes.GeographicLocation? ToIcal()
        {
            ArgumentNullException.ThrowIfNull(geolocation, nameof(geolocation));
            
            return new Ical.Net.DataTypes.GeographicLocation(geolocation.Latitude, geolocation.Longitude);
        }
    }

    extension(Ical.Net.DataTypes.GeographicLocation geolocation)
    {
        public Model.GeographicPosition ToModel()
        {
            ArgumentNullException.ThrowIfNull(geolocation, nameof(geolocation));
            
            return new GeographicPosition(geolocation.Latitude, geolocation.Longitude);
        }
    }
    
    extension(RecurrenceRuleParts.WeekDay weekDay)
    {
        Ical.Net.DataTypes.WeekDay ToIcal()
        {
            ArgumentNullException.ThrowIfNull(weekDay, nameof(weekDay));
            
            if (weekDay.Ordinal is not null)
            {
                return new Ical.Net.DataTypes.WeekDay(weekDay.Value.ToDayOfWeek(), weekDay.Ordinal.Value);
            }
            
            return new Ical.Net.DataTypes.WeekDay(weekDay.Value.ToDayOfWeek());
        }
    }

    extension(Ical.Net.DataTypes.WeekDay day)
    {
        RecurrenceRuleParts.WeekDay ToModel()
        {
            ArgumentNullException.ThrowIfNull(day, nameof(day));
            
            return new WeekDay(day.DayOfWeek.ToIsoDayOfWeek(), day.Offset is not null ? new(day.Offset.Value) : null);
        }
    }
    
    extension(Model.Attendee attendee)
    {
        Ical.Net.DataTypes.Attendee ToIcal()
        {
            ArgumentNullException.ThrowIfNull(attendee, nameof(attendee));
            
            return new Ical.Net.DataTypes.Attendee()
            {
                CommonName = attendee.CommonName,
                Value = new Uri($"EMAILTO:{attendee.Address}"),
                Role = attendee.Role switch
                {
                    Role.Chair => Ical.Net.ParticipationRole.Chair,
                    Role.NonParticipant => Ical.Net.ParticipationRole.NonParticipant,
                    Role.Optional => Ical.Net.ParticipationRole.OptionalParticipant,
                    _ => Ical.Net.ParticipationRole.RequiredParticipant,
                },
                Rsvp =  attendee.Rsvp,
                ParticipationStatus = attendee.ParticipationStatus switch
                {
                    ParticipationStatus.NeedsAction => Ical.Net.EventParticipationStatus.NeedsAction,
                    ParticipationStatus.Accepted => Ical.Net.EventParticipationStatus.Accepted,
                    ParticipationStatus.Declined => Ical.Net.EventParticipationStatus.Declined,
                    ParticipationStatus.Tentative => Ical.Net.EventParticipationStatus.Tentative,
                    ParticipationStatus.Delegated => Ical.Net.EventParticipationStatus.Delegated,
                    _ => Ical.Net.EventParticipationStatus.NeedsAction,
                },
            };
        } 
    }

    extension(Ical.Net.DataTypes.Attendee attendee)
    {
        Model.Attendee ToModel()
        {
            ArgumentNullException.ThrowIfNull(attendee, nameof(attendee));
            
            return new Attendee(
                attendee.CommonName, 
                attendee.Value.OriginalString.Replace("EMAILTO:", ""),
                false, 
                attendee.Role switch
                {
                    Ical.Net.ParticipationRole.Chair => Role.Chair,
                    Ical.Net.ParticipationRole.NonParticipant => Role.NonParticipant,
                    Ical.Net.ParticipationRole.OptionalParticipant => Role.Optional,
                    _ => Role.Required,
                }, 
                attendee.ParticipationStatus switch
                {
                    Ical.Net.EventParticipationStatus.NeedsAction => ParticipationStatus.NeedsAction,
                    Ical.Net.EventParticipationStatus.Accepted => ParticipationStatus.Accepted,
                    Ical.Net.EventParticipationStatus.Declined => ParticipationStatus.Declined,
                    Ical.Net.EventParticipationStatus.Tentative => ParticipationStatus.Tentative,
                    Ical.Net.EventParticipationStatus.Delegated => ParticipationStatus.Delegated,
                    _ => ParticipationStatus.NeedsAction,
                });
        }
    }

    extension(Model.Attachment attachment)
    {
        Ical.Net.DataTypes.Attachment ToIcal()
        {
            ArgumentNullException.ThrowIfNull(attachment, nameof(attachment));
            
            return attachment switch
            {
                Attachment.BinaryAttachment binary => new Ical.Net.DataTypes.Attachment(binary.Value),
                Attachment.UriAttachment uri => new Ical.Net.DataTypes.Attachment(uri.Value.ToString()),
                _ => throw new InvalidOperationException("Attachment must have either a value or data")
            };
        }
    }
    
    
    extension(Ical.Net.DataTypes.Attachment attachment)
    {
        Model.Attachment ToModel()
        {
            ArgumentNullException.ThrowIfNull(attachment, nameof(attachment));
            
            Model.Attachment model = attachment switch
            {
                { Data: not null } => new Attachment.BinaryAttachment(attachment.Data),
                { Uri: not null } => new Attachment.UriAttachment(attachment.Uri),
                _ => throw new InvalidOperationException("Attachment must have either a value or data")
             };

            return model;
        }
    }


    extension(Model.RecurrenceIdentifier recurrenceIdentifier)
    {
        public Ical.Net.DataTypes.RecurrenceIdentifier ToIcal()
        {
            ArgumentNullException.ThrowIfNull(recurrenceIdentifier, nameof(recurrenceIdentifier));
            
            var range = recurrenceIdentifier.Range switch
            {
                RecurrenceIdentifierRange.ThisInstance => Ical.Net.DataTypes.RecurrenceRange.ThisInstance,
                RecurrenceIdentifierRange.ThisAndFuture => Ical.Net.DataTypes.RecurrenceRange.ThisAndFuture,
                _ => throw new NotImplementedException($"Range {recurrenceIdentifier.Range} not supported")
            };
            
            return new Ical.Net.DataTypes.RecurrenceIdentifier(recurrenceIdentifier.Start.ToIcal(), range);
        }
    }

    extension(Ical.Net.DataTypes.RecurrenceIdentifier recurrenceIdentifier)
    {
        public Model.RecurrenceIdentifier ToModel()
        {
            ArgumentNullException.ThrowIfNull(recurrenceIdentifier, nameof(recurrenceIdentifier));
            
            var range = recurrenceIdentifier.Range switch
            {
                Ical.Net.DataTypes.RecurrenceRange.ThisInstance => RecurrenceIdentifierRange.ThisInstance,
                Ical.Net.DataTypes.RecurrenceRange.ThisAndFuture => RecurrenceIdentifierRange.ThisAndFuture,
                _ => throw new NotImplementedException($"Range {recurrenceIdentifier.Range} not supported")
            };
            
            return new Model.RecurrenceIdentifier(recurrenceIdentifier.StartTime.ToModel(), range);
        }
    }
    

    extension(RecurrenceRule recurrenceRule)
    {
        public Ical.Net.DataTypes.RecurrencePattern ToIcal()
        {
            ArgumentNullException.ThrowIfNull(recurrenceRule, nameof(recurrenceRule));
            
            var byDay = recurrenceRule.ByDay?.Values?.Select(d => d.ToIcal()).ToList() ?? [];
            
            return new Ical.Net.DataTypes.RecurrencePattern()
            {
                Frequency = recurrenceRule.Frequency switch
                {
                    Frequency.Secondly => Ical.Net.FrequencyType.Secondly,
                    Frequency.Minutely => Ical.Net.FrequencyType.Minutely,
                    Frequency.Hourly => Ical.Net.FrequencyType.Hourly,
                    Frequency.Daily => Ical.Net.FrequencyType.Daily,
                    Frequency.Weekly => Ical.Net.FrequencyType.Weekly,
                    Frequency.Monthly => Ical.Net.FrequencyType.Monthly,
                    Frequency.Yearly => Ical.Net.FrequencyType.Yearly,
                    _ => throw new NotImplementedException($"Frequency {recurrenceRule.Frequency} not supported")
                },
                Interval = recurrenceRule.Interval?.Value ?? 1,
                Count = recurrenceRule.UntilOrCount?.Count?.Value,
                FirstDayOfWeek = recurrenceRule.WeekStart,
                Until = recurrenceRule.UntilOrCount?.Until?.ToIcal(),
                BySecond = recurrenceRule.BySecond?.Value?.Select(x=>x.Value).ToList() ?? [],
                ByMinute = recurrenceRule.ByMinute?.Value?.Select(x=>x.Value).ToList() ?? [],
                ByHour = recurrenceRule.ByHour?.Value?.Select(x=>x.Value).ToList() ?? [],
                ByDay = byDay,
                ByWeekNo = recurrenceRule.ByWeek?.Value?.Select(x=>x.Value).ToList() ?? [],
                ByMonth = recurrenceRule.ByMonth?.Value?.Select(x=>x.Value).ToList() ?? [],
                ByMonthDay = recurrenceRule.ByMonthDay?.Value?.Select(x=>x.Value).ToList() ?? [],
                ByYearDay = recurrenceRule.ByYearDay?.Value?.Select(x=>x.Value).ToList() ?? [],
                BySetPosition = recurrenceRule.BySetPos?.Value?.Select(x=>x.Value).ToList() ?? [],
            };
        }
    }

    extension(Ical.Net.DataTypes.RecurrencePattern rule)
    {
        RecurrenceRule ToModel(Ical.Net.DataTypes.CalDateTime start)
        {
            ArgumentNullException.ThrowIfNull(rule, nameof(rule));
            
            var frequency = rule.Frequency switch
            {
                Ical.Net.FrequencyType.Secondly => Frequency.Secondly,
                Ical.Net.FrequencyType.Minutely => Frequency.Minutely,
                Ical.Net.FrequencyType.Hourly => Frequency.Hourly,
                Ical.Net.FrequencyType.Daily => Frequency.Daily,
                Ical.Net.FrequencyType.Weekly => Frequency.Weekly,
                Ical.Net.FrequencyType.Monthly => Frequency.Monthly,
                Ical.Net.FrequencyType.Yearly => Frequency.Yearly,
                _ => throw new NotImplementedException($"Frequency {rule.Frequency} not supported")
            };
            
            var interval = new Interval(rule.Interval);
            var end = rule switch
            {
                {Until:not null } => UntilOrCount.On(start.ToModel(), rule.Until!.ToModel().CalDate),
                {Count:not null } => UntilOrCount.After(new(rule.Count.Value)),
                _ => UntilOrCount.Forever
            };

            BySecond bySecond = new(rule.BySecond.Select(s=> new Second(s)));
            ByMinute byMinute = new(rule.ByMinute.Select(s => new Minute(s)));
            ByHour byHour = new(rule.ByHour.Select(h => new Hour(h)));
            ByDay byDay = new(rule.ByDay.Select(d=>d.ToModel()).ToList());
            ByWeekNo byWeekNo = new(rule.ByWeekNo.Select(w=> new WeekNo(w)));
            ByMonth byMonth = new(rule.ByMonth.Select(m => new Month(m)));
            ByMonthDay byMonthDay = new(rule.ByMonthDay.Select(d => new MonthDay(d)));
            ByYearDay byYearDay = new(rule.ByYearDay.Select(d => new YearDay(d)));
            BySetPos bySetPos = new(rule.BySetPosition.Select(d => new SetPos(d)));
            
            
            return new RecurrenceRule(frequency, interval, end, bySecond, byMinute, byHour, byDay, byWeekNo, byMonthDay, byMonth, byYearDay);
        }
    }

    extension(Model.Trigger trigger)
    {
        Ical.Net.DataTypes.Trigger ToIcal()
        {
            ArgumentNullException.ThrowIfNull(trigger, nameof(trigger));
            
            return new Ical.Net.DataTypes.Trigger
            {
                Duration = trigger.Duration?.ToIcal(),
                DateTime = trigger.DateTime?.ToIcal(),
                Related = trigger.Relation switch
                {
                    TriggerRelation.Start => Ical.Net.TriggerRelation.Start,
                    TriggerRelation.End => Ical.Net.TriggerRelation.End,
                    null => Ical.Net.TriggerRelation.Start,
                    _ => throw new ArgumentOutOfRangeException(nameof(trigger.Relation), trigger.Relation, null)
                }
            };
        }
    }
    
    extension(Ical.Net.DataTypes.Trigger trigger)
    {
        Model.Trigger ToModel()
        {
            ArgumentNullException.ThrowIfNull(trigger, nameof(trigger));

            if (trigger.IsRelative)
            {
                var relation = trigger.Related switch
                {
                    Ical.Net.TriggerRelation.Start => TriggerRelation.Start,
                    Ical.Net.TriggerRelation.End => TriggerRelation.End,
                    _ => throw new ArgumentOutOfRangeException(nameof(trigger.Related), trigger.Related, null)
                };
                
                return Trigger.Relative(trigger.Duration.ToModel(), relation);
            }
            
            return Trigger.Absolute(trigger.DateTime!.ToModel());
        }
    }

    extension(Model.Alarm alarm)
    {
        Ical.Net.CalendarComponents.Alarm ToIcal()
        {
            ArgumentNullException.ThrowIfNull(alarm, nameof(alarm));
            return new Ical.Net.CalendarComponents.Alarm
            {
                Action = alarm.Action switch
                {
                    AlarmAction.Audio => Ical.Net.AlarmAction.Audio,
                    AlarmAction.Display => Ical.Net.AlarmAction.Display,
                    AlarmAction.Email => Ical.Net.AlarmAction.Email,
                    _ => throw new NotSupportedException($"Alarm action {alarm.Action} not supported")
                },
                Trigger = alarm.Trigger.ToIcal(),
                Description = alarm.Description,
                Summary = alarm.Summary,
                Attachment = alarm.Attachment?.ToIcal(),
                Repeat = alarm.Repeat?.Value ?? 0
            };
        }
    }

    extension(Ical.Net.CalendarComponents.Alarm alarm)
    {
        Model.Alarm ToModel()
        {
            ArgumentNullException.ThrowIfNull(alarm, nameof(alarm));
            
            var trigger = alarm.Trigger?.ToModel();
            var description = alarm.Description;
            var summary = alarm.Summary;
            var attach = alarm.Attachment?.ToModel();
            var attendees = alarm.Attendees?.Select(a => a.ToModel()).ToList();
            var duration = alarm.Duration.ToModel();
            Repeat? repeat = alarm.Repeat is 0? null: new Repeat(alarm.Repeat, duration);
            
            Alarm model = alarm.Action switch
            {
                Ical.Net.AlarmAction.Audio => new Model.AudioAlarm(trigger!, attach, repeat),
                Ical.Net.AlarmAction.Display => new Model.DisplayAlarm(trigger!, description!, duration, repeat),
                Ical.Net.AlarmAction.Email => new Model.EmailAlarm(trigger!, description!, summary, attendees, repeat),
                _ => throw new NotSupportedException($"Alarm action {alarm.Action} not supported")
            };


            return model;
        }
    }
    
    extension(Model.Event @event)
    {
        public Ical.Net.CalendarComponents.CalendarEvent ToIcal()
        {
            ArgumentNullException.ThrowIfNull(@event, nameof(@event));
            
            var attendees = @event.Attendees?.Select(at => at.ToIcal()).ToList() ?? [];
            var calendarEvent =  new Ical.Net.CalendarComponents.CalendarEvent()
            {
                Uid = @event.Id.ToString(),
                Name = @event.Title!,
                Summary = @event.Title,
                Status = @event.Status switch
                {
                    EventStatus.Cancelled => Ical.Net.EventStatus.Cancelled,
                    EventStatus.Tentative => Ical.Net.EventStatus.Tentative,
                    _ => Ical.Net.EventStatus.Confirmed,
                },
                Location = @event.Location,
                GeographicLocation = @event.GeographicPosition.ToIcal(),
                End = @event.Interval?.DateTime?.ToIcal(),
                Created = @event.Created.ToIcal(),
                LastModified = @event.LastModified?.ToIcal(),
                Start = @event.Start.ToIcal(),
                Duration = @event.Interval?.Duration?.ToIcal(),
                Properties =
                {
                    new Ical.Net.CalendarProperty(nameof(Entry.CalendarId), @event.CalendarId),
                    new Ical.Net.CalendarProperty(nameof(Entry.UserId), @event.UserId)
                },
                RecurrenceRules = @event.Recurrence is not null ? [@event.Recurrence.ToIcal()!] : [],
                RecurrenceIdentifier = @event.RecurrenceIdentifier?.ToIcal(),
                Attendees = attendees,
            };
            
            var alarm = @event.Alarm?.ToIcal();

            if (alarm is not null)
            {
                calendarEvent.Alarms.Add(alarm);
            }
            
            return calendarEvent;
        }
    }
    
    extension(Ical.Net.CalendarComponents.CalendarEvent @event)
    {
        public Model.Event ToModel()
        {
            ArgumentNullException.ThrowIfNull(@event);
            
            _ = Guid.TryParse(@event.Uid,  out var id);
            var title = @event.Summary;
            var description = @event.Description;
            var start = @event.Start!.ToModel();

            TimeInterval dateTime = @event switch
            {
                { End: not null } => TimeInterval.At(@event.End.ToModel()),
                { Duration: not null } => TimeInterval.Lasts(@event.Duration.ToModel())
            };
            
            var location = @event.Location;
            var geographicPosition = @event?.GeographicLocation?.ToModel();
            var status = @event?.Status switch
            {
                Ical.Net.EventStatus.Cancelled => EventStatus.Cancelled,
                Ical.Net.EventStatus.Tentative => EventStatus.Tentative,
                _ => EventStatus.Confirmed,
            };
            var created = @event.Created.ToModel();
            var lastModified = @event.LastModified?.ToModel();
            _ = Guid.TryParse(@event.Properties[nameof(Entry.CalendarId)]?.ToString(), out var calendarId);
            _ = Guid.TryParse(@event.Properties[nameof(Entry.UserId)]?.ToString(), out var userId);
            
            var rule = @event.RecurrenceRules?.FirstOrDefault()?.ToModel(@event.Start);
            var recurrenceIdentifier = @event.RecurrenceIdentifier?.ToModel();
            var alarm = @event.Alarms?.FirstOrDefault()?.ToModel();
            var attendees = @event.Attendees.Select(at => at.ToModel()).ToImmutableHashSet();

            return new Event(userId,calendarId, title, description, start, dateTime, alarm, rule, location, geographicPosition,recurrenceIdentifier, attendees)
            {
                Created = created,
                LastModified = lastModified,
                Status = status,
            };
        }
    }

    extension(Model.Todo todo)
    {
        Ical.Net.CalendarComponents.Todo ToIcal()
        {
            ArgumentNullException.ThrowIfNull(todo, nameof(todo));
            
            var icalTodo =  new Ical.Net.CalendarComponents.Todo()
            {
                Uid = todo.Id.ToString(),
                Name = todo.Title,
                Summary = todo.Title,
                Description = todo.Description,
                Priority = todo.Priority switch
                {
                    Priority.Low => 9,
                    Priority.High => 1,
                    Priority.Medium => 5,
                    null => 0,
                    _ => throw new NotSupportedException($"Priority {todo.Priority} not supported")
                },
                Status = todo.Status switch
                {
                    Contracts.TodoStatus.Cancelled => Ical.Net.TodoStatus.Cancelled,
                    Contracts.TodoStatus.Completed => Ical.Net.TodoStatus.Completed,
                    Contracts.TodoStatus.InProcess => Ical.Net.TodoStatus.InProcess,
                    _ => Ical.Net.TodoStatus.NeedsAction,
                },
                Location = todo.Location,
                GeographicLocation = todo.GeographicPosition.ToIcal(),
                RecurrenceRules = todo.Recurrence is not null ? [todo.Recurrence.ToIcal()] : [],
                Due = todo.Due?.ToIcal(),
                Duration = todo.Interval?.Duration?.ToIcal(),
                Created = todo.Created?.ToIcal(),
                LastModified = todo.LastModified?.ToIcal(),
                Start = todo.Start?.ToIcal(),
                Completed = todo.Completed?.ToIcal(),
                Properties =
                {
                    new Ical.Net.CalendarProperty(nameof(Entry.CalendarId), todo.CalendarId),
                    new Ical.Net.CalendarProperty(nameof(Entry.UserId), todo.UserId)
                }
            };
            
            var alarm = todo.Alarm?.ToIcal();

            if (alarm is not null)
            {
                icalTodo.Alarms.Add(alarm);
            }
            
            return icalTodo;

        }
    }

    extension(Ical.Net.CalendarComponents.Todo todo)
    {
        Model.Todo ToModel()
        {
            _ = Guid.TryParse(todo.Uid, out var id);
            var title = todo.Summary;
            var description = todo.Description;
            
            TimeInterval dateTime = todo switch
            {
                { Due: not null } => TimeInterval.At(todo.Due.ToModel()),
                { Duration: not null } => TimeInterval.Lasts(todo.Duration.ToModel())
            };
            
            var location = todo.Location;
            var geographicPosition = todo.GeographicLocation?.ToModel();
            
            var priority = todo.Priority switch
            {
                1 => Priority.High,
                5 => Priority.Medium,
                9 => Priority.Low,
                _ => throw new NotSupportedException($"Priority {todo.Priority} not supported")
            };
            
            var status = todo.Status switch
            {
                Ical.Net.TodoStatus.Cancelled => TodoStatus.Cancelled,
                Ical.Net.TodoStatus.Completed => TodoStatus.Completed,
                Ical.Net.TodoStatus.InProcess => TodoStatus.InProcess,
                _ => TodoStatus.NeedsAction
            };
            
            var created = todo.Created!.ToModel();
            var lastModified = todo.LastModified?.ToModel();
            var start = todo.Start!.ToModel();
            _ = Guid.TryParse(todo.Properties[nameof(Entry.CalendarId)]?.ToString(), out var calendarId);
            _ = Guid.TryParse(todo.Properties[nameof(Entry.UserId)]?.ToString(), out var userId);
            
            var rule = todo.RecurrenceRules?.FirstOrDefault()?.ToModel(todo.Start);
            var recurrenceIdentifier = todo.RecurrenceIdentifier?.ToModel();
            
            var alarm = todo.Alarms?.FirstOrDefault()?.ToModel();
            
            var completed = todo.Completed?.ToModel();

            return new Model.Todo(userId, calendarId, title, description, start, dateTime, priority, alarm, rule, location, geographicPosition, recurrenceIdentifier)
            {
                Created = created,
                LastModified = lastModified,
                Status = status,
                Completed = completed,
            };
        }
    }

    extension(Model.Calendar calendar)
    {
        public Ical.Net.Calendar ToIcal()
        {
            var name = calendar.Name;
            var tz = calendar.TimeZone;

            var icalCalendar = new Ical.Net.Calendar { Name = name };
            icalCalendar.AddProperty("UserId", calendar.UserId.ToString());
            
            icalCalendar.TimeZones.Add(Ical.Net.CalendarComponents.VTimeZone.FromSystemTimeZone(tz));

            foreach (var @event in calendar.Events)
            {
                icalCalendar.Events.Add(@event.ToIcal());
            }

            foreach (var todo in calendar.Todos)
            {
                icalCalendar.Todos.Add(todo.ToIcal());
            }
            
            return icalCalendar;
        }
    }

    extension(Ical.Net.Calendar calendar)
    {
        public Model.Calendar ToModel()
        {
            var userId = Guid.Parse(calendar.Properties["UserId"]!.ToString());
            
            var name = calendar.Name;
            //var tz =  TimeZoneInfo.FindSystemTimeZoneById(calendar.TimeZones.First().TzId!);
            var tz =  DateTimeZoneProviders.Tzdb[calendar.TimeZones.First().TzId!];
            
            var modelCalendar = new Model.Calendar(userId, name, tz);
            
            foreach (var @event in calendar.Events)
            {
                modelCalendar.AddEvent(@event.ToModel());
            }

            foreach (var todo in calendar.Todos)
            {
                modelCalendar.AddTodo(todo.ToModel());
            }
            

            return modelCalendar;
        }
    }

    extension(Ical.Net.DataTypes.Occurrence occurrence)
    {
        public Model.Entry ToModel()
        {
            var period = occurrence.Period;
            
            var source = occurrence.Source;
            
            source.Start = period.StartTime;
            
            return source switch
            {
                Ical.Net.CalendarComponents.CalendarEvent @event => @event.ToModel(),
                Ical.Net.CalendarComponents.Todo todo => todo.ToModel(),
                _ => throw new NotSupportedException($"Entry type {occurrence.GetType()} not supported")
            };
        }
    }

    extension(Model.Entry entry)
    {
        public Ical.Net.CalendarComponents.IRecurrable ToIcal()
        {
            return entry switch
            {
                Model.Event @event => @event.ToIcal(),
                Model.Todo todo => todo.ToIcal(),
                _ => throw new NotSupportedException($"Entry type {entry.GetType()} not supported")
            };
        }
    }
}
