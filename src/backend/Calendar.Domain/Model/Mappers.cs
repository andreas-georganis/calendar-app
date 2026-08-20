using Ical.Net;
using Ical.Net.DataTypes;

using NodaTime;

namespace Calendar.Domain.Model;

internal static class FrequencyMapper
{
    extension(Frequency frequency)
    {
        public FrequencyType ToIcal()
        {
            return frequency switch
            {
                Frequency.Secondly => FrequencyType.Secondly,
                Frequency.Minutely => FrequencyType.Minutely,
                Frequency.Hourly => FrequencyType.Hourly,
                Frequency.Daily => FrequencyType.Daily,
                Frequency.Weekly => FrequencyType.Weekly,
                Frequency.Monthly => FrequencyType.Monthly,
                Frequency.Yearly => FrequencyType.Yearly,
                _ => throw new ArgumentOutOfRangeException(nameof(frequency), frequency, null)
            };
        }
    }

    extension(FrequencyType frequency)
    {
        public Frequency ToDomain()
        {
            return frequency switch
            {
                FrequencyType.Secondly => Frequency.Secondly,
                FrequencyType.Minutely => Frequency.Minutely,
                FrequencyType.Hourly => Frequency.Hourly,
                FrequencyType.Daily => Frequency.Daily,
                FrequencyType.Weekly => Frequency.Weekly,
                FrequencyType.Monthly => Frequency.Monthly,
                FrequencyType.Yearly => Frequency.Yearly,
                _ => throw new ArgumentOutOfRangeException(nameof(frequency), frequency, null)
            };
        }
    }
}

internal static class DayOfWeekMapper
{
    extension(IsoDayOfWeek dayOfWeek)
    {
        public DayOfWeek ToIcal()
        {
            return dayOfWeek switch
            {
                IsoDayOfWeek.Sunday => DayOfWeek.Sunday,
                IsoDayOfWeek.Monday => DayOfWeek.Monday,
                IsoDayOfWeek.Tuesday => DayOfWeek.Tuesday,
                IsoDayOfWeek.Wednesday => DayOfWeek.Wednesday,
                IsoDayOfWeek.Thursday => DayOfWeek.Thursday,
                IsoDayOfWeek.Friday => DayOfWeek.Friday,
                IsoDayOfWeek.Saturday => DayOfWeek.Saturday,
                _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null)
            };
        }
    }

    extension(DayOfWeek dayOfWeek)
    {
        public IsoDayOfWeek ToDomain()
        {
            return dayOfWeek switch
            {
                DayOfWeek.Sunday => IsoDayOfWeek.Sunday,
                DayOfWeek.Monday => IsoDayOfWeek.Monday,
                DayOfWeek.Tuesday => IsoDayOfWeek.Tuesday,
                DayOfWeek.Wednesday => IsoDayOfWeek.Wednesday,
                DayOfWeek.Thursday => IsoDayOfWeek.Thursday,
                DayOfWeek.Friday => IsoDayOfWeek.Friday,
                DayOfWeek.Saturday => IsoDayOfWeek.Saturday,
                _ => throw new ArgumentOutOfRangeException(nameof(dayOfWeek), dayOfWeek, null)
            };
        }
    }
}


internal static class DateTimeMapper
{
    extension(CalDateTime dateTime)
    {
        public Ical.Net.DataTypes.CalDateTime ToIcal()
        {
            if (dateTime.Zone is not null)
            {
                return new Ical.Net.DataTypes.CalDateTime(dateTime.Date.Year, dateTime.Date.Month, dateTime.Date.Day, dateTime.Time?.Hour ?? 0, dateTime.Time?.Minute ?? 0, dateTime.Time?.Second ?? 0, dateTime.Zone.Id);
            }
            else if (dateTime.Time is not null)
            {
                return new Ical.Net.DataTypes.CalDateTime(dateTime.Date.Year, dateTime.Date.Month, dateTime.Date.Day, dateTime.Time.Value.Hour, dateTime.Time.Value.Minute, dateTime.Time.Value.Second);
            }
            else
            {
                return new Ical.Net.DataTypes.CalDateTime(dateTime.Date.Year, dateTime.Date.Month, dateTime.Date.Day);
            }
        }
    }

    extension(Ical.Net.DataTypes.CalDateTime date)
    {
        public CalDateTime ToDomain()
        {
            var localDate = new LocalDate(date.Year, date.Month, date.Day);
            if (date.HasTime)
            {
                var localTime = new LocalTime(date.Hour, date.Minute, date.Second);
                if (!string.IsNullOrEmpty(date.TzId))
                {
                    var zone = DateTimeZoneProviders.Tzdb[date.TzId];
                    return new CalDateTime(localDate, localTime, zone);
                }
                else
                {
                    return new CalDateTime(localDate, localTime);
                }
            }
            else
            {
                return new CalDateTime(localDate);
            }
        }
    }
}

internal static class OrdinalWeekDayMapper
{
    extension(OrdinalWeekDay ordinalWeekDay)
    {
        public WeekDay ToIcal()
        {
            return ordinalWeekDay.Ordinal.HasValue? 
                new WeekDay(ordinalWeekDay.Value.ToIcal(), ordinalWeekDay.Ordinal.Value) : 
                new WeekDay(ordinalWeekDay.Value.ToIcal());
        }
    }

    extension(WeekDay weekDay)
    {
        public OrdinalWeekDay ToDomain()
        {
            return new OrdinalWeekDay(weekDay.DayOfWeek.ToDomain(), 
            weekDay.Offset.HasValue ? new YearWeek(weekDay.Offset.Value) : null);
        }
    }
}

internal static class RecurrenceRuleMapper
{
    extension(RecurrenceRule rule)
    {
        internal RecurrencePattern ToIcal()
        {
            var icalRule = new Ical.Net.DataTypes.RecurrencePattern
            {
                Frequency = rule.Frequency.ToIcal(),
                Interval = rule.Interval.Value,
                Count = rule.Count?.Value,
                Until = rule.Until?.ToIcal(),
                ByDay = rule.ByDay?.Value.Select(d => d.ToIcal()).ToList() ?? [],
                ByMonthDay = rule.ByMonthDay?.Value.Select(d => d.Value).ToList()?? [],
                ByMonth = rule.ByMonth?.Value.Select(d => d.Value).ToList()?? [],
                ByYearDay = rule.ByYearDay?.Value.Select(d => d.Value).ToList()?? [],
                ByWeekNo = rule.ByWeek?.Value.Select(d => d.Value).ToList()?? [],
                BySetPosition = rule.BySetPos?.Value.Select(d => d.Value).ToList()?? [],
                FirstDayOfWeek = rule.WeekStart.ToIcal()
            };

            return icalRule;
        }
    }

    extension(RecurrencePattern pattern)
    {
        public RecurrenceRule ToDomain()
        {
            var rule = new RecurrenceRule(
                pattern.Frequency.ToDomain(),
                new Interval(pattern.Interval),
                pattern.Until?.ToDomain(),
                pattern.Count.HasValue ? new Count(pattern.Count.Value) : null,
                pattern.BySecond is not null ? new(pattern.BySecond.Select(d => new Second(d)).ToList()) : null,
                pattern.ByMinute is not null ? new(pattern.ByMinute.Select(d => new Minute(d)).ToList()) : null,
                pattern.ByHour is not null ? new(pattern.ByHour.Select(d => new Hour(d)).ToList()) : null,
                pattern.ByDay is not null ? new(pattern.ByDay.Select(d => d.ToDomain()).ToList()) : null,
                pattern.ByWeekNo is not null ? new(pattern.ByWeekNo.Select(d => new YearWeek(d)).ToList()) : null,
                pattern.ByMonthDay is not null ? new(pattern.ByMonthDay.Select(d => new MonthDay(d)).ToList()) : null,
                pattern.ByMonth is not null ? new(pattern.ByMonth.Select(d => new Month(d)).ToList()) : null,
                pattern.ByYearDay is not null ? new(pattern.ByYearDay.Select(d => new YearDay(d)).ToList()) : null,
                pattern.BySetPosition is not null ? new(pattern.BySetPosition.Select(d => new SetPos(d)).ToList()) : null
            );

            return rule;
        }
    }
}

internal static class CalendarMapper
{
    extension(Domain.Model.Calendar calendar)
    {
        public Ical.Net.Calendar ToIcal()
        {
            var icalCalendar = new Ical.Net.Calendar
            {
                
            };

            return icalCalendar;
        }
    }

    extension(Ical.Net.Calendar ical)
    {
        public Domain.Model.Calendar ToDomain()
        {
            Domain.Model.Calendar calendar = default;

            return calendar;
        }
    }
}

internal static class RecurrableMapper
{
    extension(Ical.Net.CalendarComponents.IRecurrable ical)
    {
        public object ToDomain()
        {
            return ical switch
            {
                Ical.Net.CalendarComponents.CalendarEvent @event => @event.ToDomain(),
                Ical.Net.CalendarComponents.Todo todo => todo.ToDomain(),
                _ => throw new ArgumentOutOfRangeException(nameof(ical), ical, null)
            };
        }
    }
}

internal static class TodoMapper
{
    extension(Todo todo)
    {
        public Ical.Net.CalendarComponents.Todo ToIcal()
        {
            var icalTodo = new Ical.Net.CalendarComponents.Todo
            {
                
            };

            return icalTodo;
        }
    }

    extension(Ical.Net.CalendarComponents.Todo ical)
    {
        public Todo ToDomain()
        {
            Todo todo = default;

            return todo;
        }
    }
}

internal static class EventMapper
{
    extension(Event @event)
    {
        public Ical.Net.CalendarComponents.CalendarEvent ToIcal()
        {
            var icalEvent = new Ical.Net.CalendarComponents.CalendarEvent
            {
                
            };

            return icalEvent;
        }
    }

    extension(Ical.Net.CalendarComponents.CalendarEvent ical)
    {
        public Event ToDomain()
        {
            Event @event = default;

            return @event;
        }
    }
}