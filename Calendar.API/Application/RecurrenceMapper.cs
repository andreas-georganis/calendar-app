using System.Collections.Immutable;
using CalendarApp.API.Model.RecurrenceRuleParts;

namespace CalendarApp.API.Application;

public static class RecurrenceMapper
{
    extension(Contracts.Recurrence recurrence)
    {
        public Model.Recurrence ToModel(Model.DateTime start)
        {
            return default;
        }
    }

    extension(Model.Recurrence recurrence)
    {
        public Contracts.Recurrence ToContract()
        {
            return default;
        }
    }
    
    
    extension(Contracts.RecurrenceRule rule)
    {
        public Model.RecurrenceRule ToModel(Model.DateTime start)
        {
            var interval = rule.Interval is not null ? new Interval(rule.Interval.Value) : null;

            UntilOrCount untilOrCount = rule.UntilOrCount switch
            {
                Contracts.UntilOrCount.Count count => UntilOrCount.After(new Count(count.Value)),
                Contracts.UntilOrCount.Until until => UntilOrCount.On(start, until.Value.ToModel()),
                _ => UntilOrCount.Forever,
            };
            
            var bySecond = rule.BySecond is not null ? new BySecond(rule.BySecond.Select(s=>new Second(s))) : null; 
            var byMinute = rule.ByMinute is not null ? new ByMinute(rule.ByMinute.Select(m=> new Minute(m))) : null;
            var byHour = rule.ByHour is not null? new ByHour(rule.ByHour.Select(h=> new Hour(h))) : null;
            var byDay = rule.ByDay is not null? new ByDay(rule.ByDay.Select(d=>new WeekDay(d.Value, d.Ordinal is not null? new(d.Ordinal.Value): null)).ToImmutableList()) : null;
            var byWeek = rule.ByWeek is not null? new ByWeekNo(rule.ByWeek.Select(w=> new WeekNo(w))) : null;
            var byMonthDay = rule.ByMonthDay is not null? new ByMonthDay(rule.ByMonthDay.Select(x=>new MonthDay(x)).ToList()) : null;
            var byMonth = rule.ByMonth is not null? new ByMonth(rule.ByMonth.Select(x=>new Month(x)).ToList()) : null;
            var byYearDay = rule.ByYearDay is not null? new ByYearDay(rule.ByYearDay.Select(x=>new YearDay(x)).ToList()) : null;
            var bySetPos = rule.BySetPos is not null? new BySetPos(rule.BySetPos.Select(p=> new SetPos(p))) : null;
            
            return new Model.RecurrenceRule(
                rule.Frequency,
                interval,
                untilOrCount,
                bySecond,
                byMinute,
                byHour,
                byDay,
                byWeek,
                byMonthDay,
                byMonth,
                byYearDay,
                bySetPos
            );
        }
        
    }

    extension(Model.RecurrenceRule recurrenceRule)
    {
        public Contracts.RecurrenceRule ToContract()
        {
            return new Contracts.RecurrenceRule()
            {
                Frequency = recurrenceRule.Frequency,
                Interval = recurrenceRule.Interval?.Value,
                UntilOrCount = recurrenceRule.UntilOrCount switch
                {
                    { Until: { } until } => new Contracts.UntilOrCount.Until{ Value = until.ToContract(),},
                    { Count: { } count } => new Contracts.UntilOrCount.Count{ Value = count.Value,},
                    _ => null,
                },
                BySecond = recurrenceRule.BySecond?.Value.Select(x=>x.Value).ToImmutableList(),
                ByMinute = recurrenceRule.ByMinute?.Value.Select(x=>x.Value).ToImmutableList(),
                ByHour = recurrenceRule.ByHour?.Value.Select(x=>x.Value).ToImmutableList(),
                ByDay = recurrenceRule.ByDay?.Values.Select(d=>new Contracts.WeekDay(d.Value, d.Ordinal?.Value)).ToImmutableList(),
                ByWeek = recurrenceRule.ByWeek?.Value.Select(x=>x.Value).ToImmutableList(),
                ByMonthDay = recurrenceRule.ByMonthDay?.Value.Select(x=>x.Value).ToImmutableList(),
                ByMonth = recurrenceRule.ByMonth?.Value.Select(x=>x.Value).ToImmutableList(),
                ByYearDay = recurrenceRule.ByYearDay?.Value.Select(x=>x.Value).ToImmutableList(),
                BySetPos = recurrenceRule.BySetPos?.Value.Select(x=>x.Value).ToImmutableList(),
            };
        }
    }
}
